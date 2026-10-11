using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Models;

namespace TravelDemo.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        // Seed Admin User
        var adminUser = await userManager.FindByEmailAsync("admin@wanderlust.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@wanderlust.com",
                Email = "admin@wanderlust.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Demo Customer User
        var demoUser = await userManager.FindByEmailAsync("alex@traveler.com");
        if (demoUser == null)
        {
            demoUser = new IdentityUser
            {
                UserName = "alex@traveler.com",
                Email = "alex@traveler.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(demoUser, "Traveler123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoUser, "Customer");
            }
        }

        // Seed Destinations & Packages if empty
        if (!await context.Destinations.AnyAsync())
        {
            var destinations = new List<Destination>
            {
                new Destination
                {
                    Name = "Bali & Nusa Penida",
                    Country = "Indonesia",
                    Region = "Asia",
                    Description = "Tropical paradise with emerald rice terraces, sacred temples, crystal clear waters, and vibrant island culture.",
                    ImageUrl = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?auto=format&fit=crop&w=1200&q=80"
                },
                new Destination
                {
                    Name = "Swiss Alps & Interlaken",
                    Country = "Switzerland",
                    Region = "Europe",
                    Description = "Majestic snow-capped peaks, alpine lakes, glacier train journeys, and breathtaking outdoor adventures.",
                    ImageUrl = "https://images.unsplash.com/photo-1530122037265-a5f1f91d3b99?auto=format&fit=crop&w=1200&q=80"
                },
                new Destination
                {
                    Name = "Kyoto & Mount Fuji",
                    Country = "Japan",
                    Region = "Asia",
                    Description = "Ancient shrines, serene bamboo forests, vibrant cherry blossoms, and traditional Japanese hospitality.",
                    ImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?auto=format&fit=crop&w=1200&q=80"
                },
                new Destination
                {
                    Name = "Santorini & Mykonos",
                    Country = "Greece",
                    Region = "Europe",
                    Description = "Iconic whitewashed cliffside villages, Aegean sunsets, volcanic beach coves, and rich Mediterranean dining.",
                    ImageUrl = "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?auto=format&fit=crop&w=1200&q=80"
                },
                new Destination
                {
                    Name = "Serengeti & Zanzibar",
                    Country = "Tanzania",
                    Region = "Africa",
                    Description = "Thrilling Big Five wildlife safaris in the boundless Serengeti combined with turquoise Spice Island beaches.",
                    ImageUrl = "https://images.unsplash.com/photo-1516426122078-c23e76319801?auto=format&fit=crop&w=1200&q=80"
                },
                new Destination
                {
                    Name = "Patagonia & Glaciers",
                    Country = "Chile & Argentina",
                    Region = "Americas",
                    Description = "Untamed wilderness, dramatic granite towers, calving blue glaciers, and world-class trekking routes.",
                    ImageUrl = "https://images.unsplash.com/photo-1527004013197-933c4bb611b3?auto=format&fit=crop&w=1200&q=80"
                }
            };

            await context.Destinations.AddRangeAsync(destinations);
            await context.SaveChangesAsync();

            // Seed Packages
            var packages = new List<TravelPackage>
            {
                new TravelPackage
                {
                    DestinationId = destinations[0].Id,
                    Title = "Ultimate Bali & Nusa Penida Island Odyssey",
                    Summary = "Experience sacred water temples, secret waterfalls, floating breakfasts, snorkeling with manta rays, and sunsets over Uluwatu.",
                    DurationDays = 8,
                    PricePerPerson = 1299.00m,
                    MaxGroupSize = 10,
                    Category = PackageCategory.Beach,
                    Rating = 4.9,
                    IsFeatured = true,
                    ImageUrl = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "Luxury Villa Accommodation, Daily Organic Breakfast, Private Speedboat Transfers, Guided Manta Ray Snorkeling, Temple Entrance Fees, airport Transfers",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Arrival in Seminyak & Welcome Sunset Dinner", Description = "Check in to your private pool villa and join your expedition leader for welcome cocktails on the beach." },
                        new ItineraryDay { DayNumber = 2, Title = "Ubud Sacred Monkey Forest & Rice Terraces", Description = "Explore Tegallalang rice terraces, try the jungle swing, and visit traditional artisan villages in Ubud." },
                        new ItineraryDay { DayNumber = 3, Title = "Tegenungan Waterfall & Tirta Empul Temple", Description = "Receive a traditional spiritual cleansing blessing at Tirta Empul holy spring temple." },
                        new ItineraryDay { DayNumber = 4, Title = "Speedboat to Nusa Penida & Kelingking T-Rex Beach", Description = "Cruise across to Nusa Penida to marvel at the iconic T-Rex coastline view and Broken Beach." },
                        new ItineraryDay { DayNumber = 5, Title = "Manta Bay Snorkeling & Diamond Beach", Description = "Swim alongside gentle giant Manta Rays and walk down the limestone cliff stairs of Diamond Beach." },
                        new ItineraryDay { DayNumber = 6, Title = "Uluwatu Cliff Temple & Kecak Fire Dance", Description = "Perched on 70-meter cliffs, watch the mesmerising Kecak Fire Dance as the sun sets over the Indian Ocean." },
                        new ItineraryDay { DayNumber = 7, Title = "Relaxing Balinese Massage & Beach Club Cabana", Description = "Indulge in a 2-hour traditional Balinese spa session and VIP cabana access at Sundara Beach Club." },
                        new ItineraryDay { DayNumber = 8, Title = "Farewell Souvenir Shopping & Departure", Description = "Enjoy a final floating breakfast before private transfer to Ngurah Rai International Airport." }
                    }
                },
                new TravelPackage
                {
                    DestinationId = destinations[1].Id,
                    Title = "Swiss Alpine Wonders & Glacier Express Expedition",
                    Summary = "Ride panoramic mountain trains, scale the Jungfraujoch Top of Europe, hike alpine meadows, and taste world-class Swiss chocolates.",
                    DurationDays = 7,
                    PricePerPerson = 2450.00m,
                    MaxGroupSize = 12,
                    Category = PackageCategory.Adventure,
                    Rating = 4.95,
                    IsFeatured = true,
                    ImageUrl = "https://images.unsplash.com/photo-1530122037265-a5f1f91d3b99?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "1st Class Swiss Travel Pass, 4-Star Boutique Mountain Hotels, Daily Gourmet Breakfasts, Jungfraujoch Cable Car Ticket, Glacier Express Reserved Seats, Fondue Tasting",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Arrival in Zurich & Transfer to Lucerne", Description = "Board the scenic train along Lake Lucerne and stroll across the historic Chapel Bridge." },
                        new ItineraryDay { DayNumber = 2, Title = "Mount Pilatus Dragon Ride & Steamboat Cruise", Description = "Ascend the steepest cogwheel railway in the world to Mount Pilatus for sweeping alpine views." },
                        new ItineraryDay { DayNumber = 3, Title = "Interlaken & Lauterbrunnen Valley of 72 Waterfalls", Description = "Journey to Lauterbrunnen valley, surrounded by dramatic rock faces and roaring waterfalls." },
                        new ItineraryDay { DayNumber = 4, Title = "Jungfraujoch - Top of Europe & Ice Palace", Description = "Ascend Europe's highest railway station at 3,454m to witness the Aletsch Glacier." },
                        new ItineraryDay { DayNumber = 5, Title = "Glacier Express Panoramic Journey to Zermatt", Description = "Board the world's slowest express train across 291 bridges and 91 tunnels to Zermatt." },
                        new ItineraryDay { DayNumber = 6, Title = "Matterhorn Gornergrat Railway & Sunset Fondue", Description = "Marvel at the majestic pyramid peak of the Matterhorn and enjoy authentic Swiss cheese fondue." },
                        new ItineraryDay { DayNumber = 7, Title = "Departure from Geneva", Description = "Morning alpine stroll before scenic rail transfer to Geneva airport for departure flights." }
                    }
                },
                new TravelPackage
                {
                    DestinationId = destinations[2].Id,
                    Title = "Cultural Immersion: Kyoto Shrines & Mt. Fuji Wonders",
                    Summary = "Step into timeless Japan: tea ceremonies, bamboo groves, Bullet Train rides, geothermal hot springs, and Mt. Fuji views.",
                    DurationDays = 10,
                    PricePerPerson = 2890.00m,
                    MaxGroupSize = 14,
                    Category = PackageCategory.Culture,
                    Rating = 4.88,
                    IsFeatured = true,
                    ImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "7-Day JR Shinkansen Pass, Traditional Ryokan Stay with Onsen Baths, Kaiseki Multi-Course Dinners, Tea Ceremony Masterclass, Guided Kyoto Temple Pass, Luggage Transfers",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Tokyo Arrival & Shinjuku Neon Night Walk", Description = "Land in Tokyo, transfer to hotel, and explore Shinjuku's lantern-lit Omoide Yokocho." },
                        new ItineraryDay { DayNumber = 2, Title = "Asakusa Senso-ji & Meiji Shrine", Description = "Visit Tokyo's oldest temple, walk through Harajuku, and experience a matcha tea ceremony." },
                        new ItineraryDay { DayNumber = 3, Title = "Hakone Onsen & Mt. Fuji Cable Car", Description = "Travel to Hakone, ride Lake Ashi pirate boat, and relax in natural hot spring waters." },
                        new ItineraryDay { DayNumber = 4, Title = "Bullet Train (Shinkansen) to Ancient Kyoto", Description = "Speed at 300 km/h to Kyoto and stroll through the Gion geisha district at dusk." },
                        new ItineraryDay { DayNumber = 5, Title = "Fushimi Inari 10,000 Torii Gates & Kinkaku-ji", Description = "Hike through orange torii gate tunnels and marvel at the shimmering Golden Pavilion." },
                        new ItineraryDay { DayNumber = 6, Title = "Arashiyama Bamboo Grove & Tenryu-ji Temple", Description = "Walk through towering bamboo stalks and feed wild macaques at Iwatayama Monkey Park." },
                        new ItineraryDay { DayNumber = 7, Title = "Day Trip to Nara Deer Park & Todai-ji Great Buddha", Description = "Bow to free-roaming sacred sika deer and visit the world's largest wooden building." },
                        new ItineraryDay { DayNumber = 8, Title = "Osaka Street Food Tour (Dotonbori)", Description = "Taste takoyaki, okonomiyaki, and wagyu beef skewers along the vibrant neon canals." },
                        new ItineraryDay { DayNumber = 9, Title = "Traditional Ryokan Stay & Kaiseki Feast", Description = "Sleep on tatami mats, wear yukata robes, and relish a 10-course seasonal kaiseki dinner." },
                        new ItineraryDay { DayNumber = 10, Title = "Farewell Japan & Osaka Kansai Departure", Description = "Final Japanese craft shopping before departure flight from Osaka Kansai Airport." }
                    }
                },
                new TravelPackage
                {
                    DestinationId = destinations[3].Id,
                    Title = "Aegean Dream: Santorini Caldera & Mykonos Sunshine",
                    Summary = "Unwind in luxury whitewashed villas, cruise around volcanic calderas, taste Assyrtiko wines, and dance under Greek island skies.",
                    DurationDays = 7,
                    PricePerPerson = 2100.00m,
                    MaxGroupSize = 8,
                    Category = PackageCategory.Luxury,
                    Rating = 4.92,
                    IsFeatured = false,
                    ImageUrl = "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "5-Star Caldera Suite with Jacuzzi, High-Speed SeaJet Ferries, Private Catamaran Sunset Sailing, Wine Tasting at Venetsanos Winery, Daily Greek Breakfast Buffet",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Santorini Arrival & Oia Cliff Sunset", Description = "Check in to cliffside suite overlooking the blue-domed churches of Oia." },
                        new ItineraryDay { DayNumber = 2, Title = "Volcanic Caldera Luxury Catamaran Cruise", Description = "Sail to Red Beach, White Beach, and swim in geothermal hot springs with open bar barbecue." },
                        new ItineraryDay { DayNumber = 3, Title = "Pyrgos Castle & Assyrtiko Winery Experience", Description = "Tour medieval Pyrgos hilltop village and sample crisp volcanic wines at sunset." },
                        new ItineraryDay { DayNumber = 4, Title = "Fast Ferry to Mykonos & Little Venice Walk", Description = "Speed across the Aegean to Mykonos and enjoy cocktails next to Little Venice windmills." },
                        new ItineraryDay { DayNumber = 5, Title = "Delos Sacred Island Archaeological Tour", Description = "Sail to the uninhabited sanctuary island of Delos, mythical birthplace of Apollo." },
                        new ItineraryDay { DayNumber = 6, Title = "Psarou Beach Club Day & Sunset Dining", Description = "Lounge on sunbeds at pristine beach clubs and feast on fresh Mediterranean seafood." },
                        new ItineraryDay { DayNumber = 7, Title = "Mykonos Departure", Description = "Morning espresso overlooking the harbor before flight or ferry connection." }
                    }
                },
                new TravelPackage
                {
                    DestinationId = destinations[4].Id,
                    Title = "Serengeti Great Migration & Zanzibar Spice Paradise",
                    Summary = "Witness millions of wildebeest crossing the savannah, sleep under starry African skies, and unwind on powdery white Spice Island shores.",
                    DurationDays = 9,
                    PricePerPerson = 3600.00m,
                    MaxGroupSize = 6,
                    Category = PackageCategory.Adventure,
                    Rating = 4.98,
                    IsFeatured = true,
                    ImageUrl = "https://images.unsplash.com/photo-1516426122078-c23e76319801?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "Pop-up Roof 4x4 Land Cruisers, Luxury Tented Safari Camp, All National Park & Crater Fees, Internal Bush Flights, Zanzibar Beach Resort, Professional Ranger Guide",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Arusha Arrival & Safari Briefing", Description = "Gather with expert wildlife rangers at the lodge for safari orientation." },
                        new ItineraryDay { DayNumber = 2, Title = "Tarangire National Park Elephant Safari", Description = "Drive amongst ancient baobab trees and encounter giant herds of African elephants." },
                        new ItineraryDay { DayNumber = 3, Title = "Ngorongoro Crater Floor Big Five Expedition", Description = "Descend 600m into the extinct caldera, home to black rhinos, lions, and flamingos." },
                        new ItineraryDay { DayNumber = 4, Title = "Serengeti Endless Plains & Big Cat Tracking", Description = "Track cheetahs, leopards, and pride lions roaming the vast Serengeti grasslands." },
                        new ItineraryDay { DayNumber = 5, Title = "Great Migration River Crossing & Bush Camp", Description = "Witness dramatic river crossings of wildebeest and zebras before night campfire." },
                        new ItineraryDay { DayNumber = 6, Title = "Bush Flight to Zanzibar Spice Island", Description = "Fly directly from Seronera airstrip to Zanzibar; check in to beachside resort." },
                        new ItineraryDay { DayNumber = 7, Title = "Stone Town UNESCO Tour & Spice Plantation", Description = "Walk through Arabian alleyways, carved wooden doors, and smell fresh clove plantations." },
                        new ItineraryDay { DayNumber = 8, Title = "Safari Blue Dhow Boat & Coral Reef Snorkeling", Description = "Sail on a traditional wooden dhow, swim in sandbars, and feast on grilled lobster." },
                        new ItineraryDay { DayNumber = 9, Title = "Zanzibar Departure", Description = "Final beach sunrise walk before transfer to Zanzibar International Airport." }
                    }
                },
                new TravelPackage
                {
                    DestinationId = destinations[5].Id,
                    Title = "Patagonia Trekking: Torres del Paine & Perito Moreno",
                    Summary = "Trek to the iconic granite horns of Torres del Paine, listen to roaring glacier ice calving, and conquer Southern Patagonia.",
                    DurationDays = 8,
                    PricePerPerson = 2750.00m,
                    MaxGroupSize = 10,
                    Category = PackageCategory.Adventure,
                    Rating = 4.91,
                    IsFeatured = false,
                    ImageUrl = "https://images.unsplash.com/photo-1527004013197-933c4bb611b3?auto=format&fit=crop&w=1200&q=80",
                    Inclusions = "Certified Mountain Guides, EcoCamp Eco-Dome Lodging, All Park Permits & Catamaran Boat Passes, Ice Trekking Equipment, All Meals & Trekking Rations",
                    Itinerary = new List<ItineraryDay>
                    {
                        new ItineraryDay { DayNumber = 1, Title = "Punta Arenas to Torres del Paine Park", Description = "Drive through Patagonian pampas, spotting guanacos and rhea ostriches." },
                        new ItineraryDay { DayNumber = 2, Title = "Trek to the Base of the Towers (Torres)", Description = "Conquer the legendary 18km hike up Ascencio Valley to the turquoise glacial tarn." },
                        new ItineraryDay { DayNumber = 3, Title = "French Valley & Cuernos Granite Peaks", Description = "Hike into the amphitheater of hanging glaciers and towering granite horns." },
                        new ItineraryDay { DayNumber = 4, Title = "Grey Glacier Navigation & Ice Walking", Description = "Board boat past floating icebergs and strap on crampons for a walk on Grey Glacier." },
                        new ItineraryDay { DayNumber = 5, Title = "Border Crossing to El Calafate (Argentina)", Description = "Cross the Patagonian steppe into Argentina along Lake Argentino." },
                        new ItineraryDay { DayNumber = 6, Title = "Perito Moreno Glacier Ice Walk & Boardwalks", Description = "Hear the thunderous crack of ice walls crashing into the lake at Perito Moreno." },
                        new ItineraryDay { DayNumber = 7, Title = "El Chalten Fitz Roy Viewpoint Trek", Description = "Trek to Laguna de los Tres for majestic views of Mount Fitz Roy." },
                        new ItineraryDay { DayNumber = 8, Title = "El Calafate Departure", Description = "Transfer to El Calafate airport for departure connection." }
                    }
                }
            };

            await context.Packages.AddRangeAsync(packages);
            await context.SaveChangesAsync();

            // Seed Departures for each package (3 future departure dates per package)
            var today = DateOnly.FromDateTime(DateTime.Today);
            var departures = new List<Departure>();

            foreach (var pkg in packages)
            {
                departures.Add(new Departure
                {
                    TravelPackageId = pkg.Id,
                    StartDate = today.AddDays(14),
                    SeatsTotal = pkg.MaxGroupSize,
                    SeatsBooked = 3
                });

                departures.Add(new Departure
                {
                    TravelPackageId = pkg.Id,
                    StartDate = today.AddDays(45),
                    SeatsTotal = pkg.MaxGroupSize,
                    SeatsBooked = 1
                });

                departures.Add(new Departure
                {
                    TravelPackageId = pkg.Id,
                    StartDate = today.AddDays(90),
                    SeatsTotal = pkg.MaxGroupSize,
                    SeatsBooked = 0
                });
            }

            await context.Departures.AddRangeAsync(departures);
            await context.SaveChangesAsync();

            // Seed a sample booking for demoUser
            if (demoUser != null)
            {
                var sampleDep = departures[0];
                var sampleBooking = new TravelBooking
                {
                    DepartureId = sampleDep.Id,
                    UserId = demoUser.Id,
                    Travellers = 2,
                    TotalPrice = sampleDep.TravelPackage != null ? sampleDep.TravelPackage.PricePerPerson * 2 : 2598.00m,
                    LeadName = "Alex Traveler",
                    Phone = "+1 (555) 234-5678",
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                };
                await context.Bookings.AddAsync(sampleBooking);
                await context.SaveChangesAsync();
            }
        }
    }
}
