using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Favourite;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using GTranslate.Translators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTranslate.Translators;

namespace Service
{
    public class EtsyScrapperService : IEtsyScrapperService
    {
        private readonly ICloudinaryService _cloudinaryService;

        private readonly AggregateTranslator _translator = new();
        private readonly IGenericRepository<ProductCategory, int> _categoryRepo;
        private readonly IGenericRepository<Product, int> _productRepo;
        private readonly IGenericRepository<Tag, int> _tagRepo;
        private readonly IGenericRepository<UserInteraction, int> _interactionRepo;
        private readonly IFavouriteRepository _favouriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Dictionary<string, string> CategoryMapping = new()
        {
            { "Jewelry & Accessories", "https://www.etsy.com/c/jewelry" },
            { "Glass Crafts", "https://www.etsy.com/c/art-and-collectibles/glass-art" },
            { "Resin Art", "https://www.etsy.com/search?q=resin+art" },
            { "Handmade Cosmetics", "https://www.etsy.com/c/bath-and-beauty/makeup-and-cosmetics" },
            { "Wall Art & Frames", "https://www.etsy.com/c/home-and-living/home-decor/wall-decor" },
            { "Other", "https://www.etsy.com/search?q=handmade+gifts" },


            { "Traditional & Cultural Crafts", "https://www.etsy.com/search?q=traditional+crafts" },
            { "Religious & Spiritual Crafts", "https://www.etsy.com/c/home-and-living/spirituality-and-religion" },
            { "Home Decor", "https://www.etsy.com/c/home-and-living/home-decor" }, 
            { "Textiles & Fabric Crafts", "https://www.etsy.com/c/craft-supplies-and-tools/fabric-and-notions" },
            { "Accessories & Gifts", "https://www.etsy.com/c/jewelry-and-accessories/accessories" },
            { "Metal Crafts", "https://www.etsy.com/search?q=metal+crafts" },
            { "Pottery & Ceramics", "https://www.etsy.com/c/art-and-collectibles/sculpture/ceramics" },
            { "Recycled & Eco Crafts", "https://www.etsy.com/search?q=eco+friendly+crafts" },
            { "Leather Products", "https://www.etsy.com/search?q=leather+goods" },
            { "Art & Paintings", "https://www.etsy.com/c/art-and-collectibles/painting" },
            { "Candles & Soaps", "https://www.etsy.com/search?q=candles+and+soaps" },
            { "Paper Crafts & Stationery", "https://www.etsy.com/c/paper-and-party-supplies/paper" },
            { "Kids & Toys", "https://www.etsy.com/c/toys-and-games" },
            { "Seasonal & Event Crafts", "https://www.etsy.com/c/paper-and-party-supplies/party-supplies" },
            { "Furniture & Woodwork", "https://www.etsy.com/c/home-and-living/furniture" },
            { "Custom & Personalized Items", "https://www.etsy.com/search?q=custom+personalized" },
            { "Handmade Kitchenware", "https://www.etsy.com/c/home-and-living/kitchen-and-dining" },
            { "Clothing & Fashion", "https://www.etsy.com/c/clothing" },
            { "Bags & Wallets", "https://www.etsy.com/c/bags-and-purses" },};
        private static readonly Dictionary<string, string> _translationCache = new();
        public EtsyScrapperService(
            
            IGenericRepository<ProductCategory, int> categoryRepo,
            IGenericRepository<Product, int> productRepo,
            IGenericRepository<Tag, int> tagRepo,
            IGenericRepository<UserInteraction, int> interactionRepo,
            IFavouriteRepository favouriteRepository,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ICloudinaryService cloudinaryService
            )
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
            _tagRepo = tagRepo;
            _interactionRepo = interactionRepo;
            _favouriteRepository = favouriteRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }
        public async Task StartScrapingAsync()
        {
            // 1. جلب الخبراء والمبتدئين فقط (تجاهلنا الـ Suppliers بناءً على طلبك)
            var experts = await _userManager.GetUsersInRoleAsync("Expert");
            var beginners = await _userManager.GetUsersInRoleAsync("Beginner");

            // دمج الفئتين دول بس عشان يكونوا بائعي المنتجات المستخرجة
            var allPotentialSellers = experts.Concat(beginners).ToList();

            if (!allPotentialSellers.Any())
            {
                Console.WriteLine("⚠️ لم يتم العثور على Experts أو Beginners! تأكدي من تشغيل SeedOneThousandUsers أولاً.");
                return;
            }

            // 2. جلب الـ Customers للتفاعل (اللايكات والريفيوهات)
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            var generatedCustomers = customers.ToList();
            Console.WriteLine("\n========================================");
            Console.WriteLine($"👥 عدد العملاء (Customers) في الداتابيز: {generatedCustomers.Count}");
            Console.WriteLine("========================================\n");

            if (generatedCustomers.Count == 0)
            {
                Console.WriteLine("⚠️ الكود هيقف لأن مفيش Customers يعملوا لايكات وريفيوهات!");
                return; // عشان توقفي الكود وتوفري وقتك لحد ما تريتي العملاء
            }

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });

            var context = await browser.NewContextAsync(new()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"
            });

            var page = await context.NewPageAsync();
            await page.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

            var random = new Random();

            foreach (var mapping in CategoryMapping)
            {
                var category = await GetOrCreateCategoryAsync(mapping.Key);

                for (int pageNum = 1; pageNum <= 8; pageNum++)
                {
                    try
                    {
                        string pageUrl = mapping.Value.Contains("?") ? $"{mapping.Value}&page={pageNum}" : $"{mapping.Value}?page={pageNum}";
                        Console.WriteLine($"🚀 Processing Category: {mapping.Key} | Page {pageNum}");

                        await page.GotoAsync(pageUrl, new() { WaitUntil = WaitUntilState.Load, Timeout = 90000 });
                        await page.EvaluateAsync("window.scrollBy(0, 800)");
                        await Task.Delay(2000);

                        var productLinks = await page.Locator("a[href*='/listing/']").EvaluateAllAsync<string[]>(
                            "elements => [...new Set(elements.map(e => e.href))].filter(link => link.includes('/listing/'))");

                        foreach (var link in productLinks.Take(30))
                        {
                            // 3. اختيار بائع عشوائي (خبير أو مبتدئ) فقط
                            var randomSeller = allPotentialSellers[random.Next(allPotentialSellers.Count)];

                            await ScrapeSingleProductAsync(page, link, category.Id, randomSeller.Id, generatedCustomers);

                            await Task.Delay(random.Next(3000, 6000));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error in {mapping.Key}: {ex.Message}");
                    }
                }
            }

            await browser.CloseAsync();
            Console.WriteLine("✅✅ DONE: Products assigned to Experts and Beginners successfully!");
        }
        private async Task ScrapeSingleProductAsync(IPage page, string productUrl, int categoryId, string sellerId, List<ApplicationUser> generatedUsers)
        {
            // 1. استخراج الـ Etsy ID للتحقق من التكرار
            var etsyId = productUrl.Split("/listing/").Last().Split('/').First();
            var isDuplicate = await _productRepo.GetFirstOrDefaultAsync(p => p.ImageUrl.Contains(etsyId));


            try
            {
                // 2. فتح الصفحة
                await page.GotoAsync(productUrl, new() { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
                await Task.Delay(1000);

                // 3. سحب البيانات الخام (العنوان، الوصف، السعر)
                var fullTitleEn = await GetTextOrEmpty(page, "h1[data-buy-box-listing-title='true']");
                if (string.IsNullOrEmpty(fullTitleEn)) return;

                var cleanTitleEn = fullTitleEn.Split(':', '-', '|', ',').First().Trim();

                var highlights = await page.Locator(".wt-list-unstyled.wt-vertical-align-middle li").AllInnerTextsAsync();
                string highlightsText = highlights.Any() ? "Highlights: " + string.Join(" | ", highlights) : "";

                var descriptionLocator = page.Locator("div[data-product-details-description-text-content], #description-text, .wt-text-body-01.wt-break-word").First;
                string rawDescriptionEn = "";
                if (await descriptionLocator.CountAsync() > 0)
                {
                    rawDescriptionEn = await descriptionLocator.EvaluateAsync<string>("el => el.textContent");
                }

                var priceText = await GetTextOrEmpty(page, "div[data-buy-box-region='price'] p.wt-text-title-03, .wt-p-be-2 .wt-text-title-03, span.wt-screen-reader-only + p");

                // 4. سحب الصورة ورفعها على Cloudinary
                var imageLocator = page.Locator("img.wt-max-width-full, img.image-main").First;
                var etsyImageUrl = (await imageLocator.CountAsync() > 0) ? await imageLocator.GetAttributeAsync("src") : "";

                string finalImageUrl = etsyImageUrl; // الافتراضي
                if (!string.IsNullOrEmpty(etsyImageUrl))
                {
                    // نستخدم الخدمة لرفع الصورة والحصول على الرابط الجديد
                    // تأكدي من عمل Inject لـ ICloudinaryService في الـ Constructor الخاص بالكلاس
                    finalImageUrl = await _cloudinaryService.UploadFromUrlAsync(etsyImageUrl, etsyId);
                }

                // 5. الترجمة
                // 5. الترجمة (نسخة محسنة)
                string titleAr = "";
                string descAr = "";

                try
                {
                    // فحص الكاش للعنوان أولاً
                    if (_translationCache.TryGetValue(cleanTitleEn, out var cachedTitle))
                    {
                        titleAr = cachedTitle;
                    }
                    else
                    {
                        var resultTitle = await _translator.TranslateAsync(cleanTitleEn, "ar", "en");
                        titleAr = resultTitle.Translation;
                        _translationCache[cleanTitleEn] = titleAr; // حفظ في الكاش
                    }

                    // ترجمة أول 150 حرف فقط من الوصف لضمان السرعة
                    string shortDescEn = rawDescriptionEn.Length > 150 ? rawDescriptionEn.Substring(0, 150) : rawDescriptionEn;

                    if (_translationCache.TryGetValue(shortDescEn, out var cachedDesc))
                    {
                        descAr = cachedDesc;
                    }
                    else
                    {
                        var resultDesc = await _translator.TranslateAsync(shortDescEn, "ar", "en");
                        descAr = resultDesc.Translation;
                        _translationCache[shortDescEn] = descAr;
                    }
                }
                catch (Exception ex)
                {
                    // Fallback في حالة الفشل التام
                    titleAr = cleanTitleEn;
                    descAr = "هذا المنتج متوفر حالياً بالوصف الإنجليزي فقط.";
                    Console.WriteLine($"⚠️ Translation skipped for {etsyId} due to Rate Limit.");
                }

                // 6. إنشاء وحفظ المنتج
                var product = new Product
                {
                    NameEn = cleanTitleEn,
                    NameAr = titleAr,
                    DescriptionEn = $"{highlightsText}\n\n{rawDescriptionEn}".Trim(),
                    DescriptionAr = descAr,
                    Price = ParsePrice(priceText),
                    ImageUrl = finalImageUrl, // هنا نضع رابط Cloudinary
                    Quantity = new Random().Next(5, 50),
                    CategoryId = categoryId,
                    SellerId = sellerId,
                    IsGenerated = true
                };

                await _productRepo.AddAsync(product);
                await _unitOfWork.SaveChanges();

                // 7. التاجات (نفس الكود الخاص بكِ)
                var stopWords = new HashSet<string> { "with", "from", "your", "gift", "that", "this", "item", "made" };
                var potentialTags = cleanTitleEn
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim().ToLower().Replace("\"", "").Replace("'", ""))
                    .Where(w => w.Length > 3 && !stopWords.Contains(w))
                    .Distinct()
                    .Take(6);

                foreach (var tagText in potentialTags)
                {
                    var tag = await _tagRepo.GetFirstOrDefaultAsync(t => t.Name == tagText);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagText };
                        await _tagRepo.AddAsync(tag);
                        await _unitOfWork.SaveChanges();
                    }

                    var itemTag = new ItemTags { ItemId = product.Id, TagId = tag.Id };
                    await _unitOfWork.GetRepository<ItemTags, int>().AddAsync(itemTag);
                }

                await _unitOfWork.SaveChanges();
                Console.WriteLine($"✅ Successfully Scraped & Uploaded: {product.NameEn}");

                await SimulateInteractionsAsync(page, product.Id, generatedUsers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error Scraping Product {etsyId}: {ex.Message}");
            }
        }
        private async Task SimulateInteractionsAsync(IPage page, int productId, List<ApplicationUser> generatedUsers)
        {
            if (!generatedUsers.Any()) return;
            var random = new Random();

            // 💡 1. نلخبط اليوزرز ونعملهم ليستة جديدة عشان ناخد منهم بالدور من غير تكرار
            var shuffledUsers = generatedUsers.OrderBy(u => random.Next()).ToList();
            int currentUserIndex = 0;

            try
            {
                // 1. النزول بالصفحة لتحميل الريفيوهات
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight / 2)");
                await Task.Delay(2000);

                // 2. سحب الريفيوهات
                var reviewElements = await page.Locator("#reviews .wt-text-body-01, p[data-review-text], #same-listing-reviews-panel p").AllAsync();
                int addedReviewsCount = 0;

                foreach (var el in reviewElements)
                {
                    if (addedReviewsCount >= 10)
                    {
                        Console.WriteLine("🛑 تم الوصول للحد الأقصى (10 ريفيوهات) للمنتج ده.");
                        break;
                    }
                    if (currentUserIndex >= shuffledUsers.Count)
                    {
                        Console.WriteLine("⚠️ [تنبيه] اليوزرز خلصوا، مش هنقدر نضيف ريفيوهات تانية للمنتج ده.");
                        break;
                    }
                    // 💡 2. لو عدد الريفيوهات المسحوبة أكبر من عدد اليوزرز الوهميين اللي عندنا، نوقف اللوب عشان منكررش
                    if (currentUserIndex >= shuffledUsers.Count)
                    {
                        Console.WriteLine("⚠️ [تنبيه] اليوزرز خلصوا، مش هنقدر نضيف ريفيوهات تانية للمنتج ده.");
                        break;
                    }

                    try
                    {
                        var txt = await el.InnerTextAsync();
                        if (string.IsNullOrWhiteSpace(txt) || txt.Length < 10) continue;

                        // 💡 3. ناخد اليوزر اللي عليه الدور، وبعدين نزود العداد عشان المرة الجاية ناخد اللي بعده
                        var selectedUserId = shuffledUsers[currentUserIndex].Id;
                        currentUserIndex++;

                        var interaction = new UserInteraction
                        {
                            ProductId = productId,
                            UserId = selectedUserId, // 💡 استخدمنا اليوزر اللي اخترناه
                            Review = txt.Trim(),
                            Rating = (short)random.Next(1, 5),
                            InteractionDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                            CreatedAt = DateTime.UtcNow // 
                        };

                        // 💡 4. نستخدم الـ Repo والـ UnitOfWork بتوعك عادي جداً
                        await _interactionRepo.AddAsync(interaction);
                        await _unitOfWork.SaveChanges(); // 🔥 الحفظ الفوري لكل ريفيو على حدة

                        addedReviewsCount++;
                        Console.WriteLine($"✅ [نجاح] تم حفظ ريفيو للمنتج رقم {productId} في الداتابيز!");
                    }
                    catch (Exception ex)
                    {
                        // لو ريفيو واحد فشل، هيطبع الخطأ ويكمل للي بعده عادي جداً
                        Console.WriteLine($"❌ [تخطي] خطأ في حفظ ريفيو محدد: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }

                // 3. إضافة المفضلات (Favourites)
                int numberOfLikes = random.Next(3, 8);
                var selectedUserIds = generatedUsers.OrderBy(x => random.Next()).Take(numberOfLikes).Select(u => u.Id);
                int addedFavsCount = 0;

                foreach (var userId in selectedUserIds)
                {
                    try
                    {
                        if (await _favouriteRepository.GetFavouriteAsync(userId, productId) == null)
                        {
                            await _favouriteRepository.AddAsync(new Favourite { UserId = userId, ProductId = productId,CreatedAt= DateTime.UtcNow });
                            await _unitOfWork.SaveChanges(); // 🔥 الحفظ الفوري لكل لايك
                            addedFavsCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ [تخطي] خطأ في حفظ إعجاب: {ex.Message}");
                    }
                }

                Console.WriteLine($"🎉 الخلاصة للمنتج {productId}: تم حفظ {addedReviewsCount} ريفيو و {addedFavsCount} إعجاب.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ عام أثناء محاولة قراءة صفحة الريفيوهات: {ex.Message}");
            }
        }
        private async Task<ProductCategory> GetOrCreateCategoryAsync(string categoryName)
        {
            var category = await _categoryRepo.GetFirstOrDefaultAsync(c => c.NameEn.ToLower() == categoryName.ToLower());
            if (category == null)
            {
                category = new ProductCategory { NameEn = categoryName, NameAr = "قسم يدوي مستخرج", image = "" };
                await _categoryRepo.AddAsync(category);
                await _unitOfWork.SaveChanges();
            }
            return category;
        }
        private async Task<string> GetTextOrEmpty(IPage page, string selector)
        {
            try
            {
                var locator = page.Locator(selector).First;
                return (await locator.CountAsync() > 0) ? await locator.InnerTextAsync() : "";
            }
            catch { return ""; }
        }
        private decimal ParsePrice(string priceText)
        {
            if (string.IsNullOrEmpty(priceText))
            {
                // بدل ما نثبت 25، ندي رقم عشوائي بين 40 و 150 كنوع من التغيير لو فشل السحب
                return (decimal)new Random().Next(40, 151);
            }

            // تنظيف النص من أي رموز زي $ أو EGP أو الفواصل
            var cleaned = new string(priceText.Where(c => char.IsDigit(c) || c == '.').ToArray());

            if (decimal.TryParse(cleaned, out var res) && res > 0)
            {
                return res;
            }

            return (decimal)new Random().Next(30, 90);
        }
    }
}
