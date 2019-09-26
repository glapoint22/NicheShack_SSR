using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Models;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedingController : ControllerBase
    {
        private readonly UserManager<Customer> userManager;
        private readonly IUnitOfWork unitOfWork;

        public SeedingController(UserManager<Customer> userManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }


        [HttpPost]
        [Route("Accounts")]
        public async Task<ActionResult> Post(List<Account> accounts)
        {
            List<Product> products = (List<Product>)await unitOfWork.Products.GetCollection();

            foreach (Account account in accounts)
            {
                Customer customer = await CreateAccount(account);


                SetProductOrders(customer.Id, products);


                await unitOfWork.Save();



                foreach (Product product in products)
                {
                    WriteReview(product, customer.Id);

                    await unitOfWork.Save();

                    RateReview(product);

                    await unitOfWork.Save();
                }


                await SetListProductsAsync(customer.ListCollaborators.FirstOrDefault(x => x.CustomerId == customer.Id && x.IsOwner == true).Id, products);

                //await unitOfWork.Save();

                await SetCollaboratorsAsync(customer.Id, products);
                
            }

            return Ok();
        }




        [HttpPost]
        [Route("Products")]
        public async Task<ActionResult> PostProducts()
        {
            List<Product> products = (List<Product>)await unitOfWork.Products.GetCollection();
            Random random = new Random();


            foreach (Product product in products)
            {
                // Price points
                int numPricePoints = random.Next(1, 5);
                List<ProductPricePoint> pricePoints = new List<ProductPricePoint>();

                for (int i = 0; i < numPricePoints; i++)
                {
                    pricePoints.Add(new ProductPricePoint
                    {
                        ProductId = product.Id,
                        Price = Math.Round(random.NextDouble() * (20 - 5) + 5, 2),
                        Description = GetPricePointDescription(i)
                    });
                }



                // Product content
                int numProductContent = random.Next(1, 6);
                List<ProductContent> productContent = new List<ProductContent>();

                for (int i = 0; i < numProductContent; i++)
                {
                    string productContentId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

                    List<PriceIndex> priceIndices = new List<PriceIndex>();
                    for (int j = 0; j < pricePoints.Count; j++)
                    {
                        if (random.Next(2) == 1)
                        {
                            priceIndices.Add(new PriceIndex
                            {
                                ProductContentId = productContentId,
                                Index = j
                            });
                        }
                    }

                    if (priceIndices.Count == 0)
                    {
                        priceIndices.Add(new PriceIndex
                        {
                            ProductContentId = productContentId,
                            Index = 0
                        });
                    }


                    ProductContent content = new ProductContent
                    {
                        Id = productContentId,
                        ProductId = product.Id,
                        ProductContentTypeId = random.Next(1, 5),
                        Title = i == 0 ? product.Title : GetProductTitle(),
                        PriceIndices = priceIndices
                    };

                    productContent.Add(content);
                }

                product.ProductPricePoints = pricePoints;

                product.ProductContent = productContent;


                product.MinPrice = pricePoints.Min(x => x.Price);
                product.MaxPrice = pricePoints.Count == 1 ? 0 : pricePoints.Max(x => x.Price);


                // Product media
                if (random.Next(2) == 1)
                {
                    int numMedia = random.Next(1, 11);
                    List<ProductMedia> productMedia = new List<ProductMedia>();
                    for (int i = 0; i < numMedia; i++)
                    {
                        string url = GetProductMedia();

                        int result = productMedia.FindIndex(x => x.Url == url && x.ProductId == product.Id);

                        if (result == -1)
                        {
                            productMedia.Add(new ProductMedia
                            {
                                ProductId = product.Id,
                                Url = url,
                                Thumbnail = "someImage.png",
                                Type = 0
                            });
                        }

                    }

                    product.ProductMedia = productMedia;
                }

                unitOfWork.Products.Update(product);

                await unitOfWork.Save();

            }



            return Ok();
        }





        private string GetPricePointDescription(int num)
        {
            string description = string.Empty;

            switch (num)
            {
                case 0:
                    description = "Single Payment of {0:C2}";
                    break;
                case 1:
                    description = "{0:C2} per Week";
                    break;
                case 2:
                    description = "3 Easy Payments of {0:C2} per Month";
                    break;
                case 3:
                    description = "{0:C2} a Year";
                    break;
            }

            return description;
        }


        private async Task<Customer> CreateAccount(Account account)
        {
            Customer customer = account.CreateCustomer();

            // Add the new customer to the database
            await userManager.CreateAsync(customer, account.Password);



            // Create the new list and add it to the database
            List newList = new List
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                Name = "Wish List",
                Description = string.Empty,
                CollaborateId = Guid.NewGuid().ToString("N").ToUpper()
            };

            unitOfWork.Lists.Add(newList);


            // Set the owner as the first collaborator of the list
            ListCollaborator collaborator = new ListCollaborator
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                ListId = newList.Id,
                IsOwner = true
            };

            unitOfWork.Collaborators.Add(collaborator);


            // Save all updates to the database
            await unitOfWork.Save();

            return customer;
        }


        private void WriteReview(Product product, string customerId)
        {
            Random random = new Random();
            int rating = random.Next(0, 6);
            string title = string.Empty;
            string text = string.Empty;

            if (rating > 0)
            {
                switch (rating)
                {
                    case 1:
                        switch (random.Next(0, 20))
                        {
                            case 0:
                                title = "Clunky and Boring";
                                text = "Took a chance given the 5 star rating and regret spending the $20 bucks. Boring is the first word that comes to mind, as I wade through the storyline and struggle to figure out which button and/or stick controls which action. I guess there are those who this type of post-apocalyptic struggle to survive warn out genre appeals to, but if you are going to live in that world, then get on with the shooting and maiming and limit the half-cocked storyline.";
                                break;
                            case 1:
                                title = "Bad Quality All Around";
                                text = "Not sure why these are advertised as professional grade, because they are anything but. They feel like the computer lab headphones I used as a kid in the 90s. They do not fit over the ears unless you have very small ears, they are also not deep enough for an over-ear headphone. I found them to be stiff and uncomfortable. The heaviest part of the headphones are the cord. I'd shop around for something better if you're interested in comfort. I regret purchasing these headphones and have requested a refund.";
                                break;
                            case 2:
                                title = "Really Sony?";
                                text = "After a few tries with low-priced earbuds, and after owning an excellent set of beats by Dre earbuds that are now failing, I figured I'de go for the professional sound. I was SURE to get full sound and bass with professional headphones...........NOT...........honestly, they sound no better than the tin-sounding headphones that I've wasted money on up to this point. I compared them with the Betron B25 noise canceling headphones that cost $13.99, and there was no comparison. Try it yourself and see. Sony should be ashamed of themselves for being in the business for so long and putting out such horrible sounding product.";
                                break;
                            case 3:
                                title = "Unhappy customer";
                                text = "It’s not even close to what you see in the product description. Low quality material.";
                                break;
                            case 4:
                                title = "I don't leave reviews often but this was shocking after seeing great reviews";
                                text = "I purchased these headphones because while working out daily, the head piece around my Older headphones started to pill away from sweat so I just wanted a whole new pair of headphones so I felt after reading comments that these would be close to or even better than my 40 dollar head phones that were falling apart. Well I compared them an I was stunned and disappointed. I had to send them back and maybe think about crazy gluing my original headset which sounds better.";
                                break;
                            case 5:
                                title = "Didn't even last 6 months";
                                text = "Audio stopped consistently coming through both sides shortly after the warranty expired and I would have to fiddle with the cord to get it to come through both ears. Now 6 months after purchasing I can't even get it to come through the right ear.";
                                break;
                            case 6:
                                title = "Low quality parts - gold plated plug 1/8\" mini - jack broke(snapped) after the 1st 4 months of use.";
                                text = "Low-quality components/parts, very disappointing, gold plated plug 1/8\" mini - jack broke(snapped) after little use.I guess we have come to expect lower quality from Sony over the past 20 years. (this set was a replacement for another Sony hi - end headset wifi enabled that also broke due to low - quality parts / manufacturing) Otherwise, they do have a great sound and super comfy.if you consider them as discardable headphones they work fine....";
                                break;
                            case 7:
                                title = "Buzzz";
                                text = "BUZZZZZ Sound quality is bad.";
                                break;
                            case 8:
                                title = "Sound quality was ok but a fom ear piece fell off at 7 months";
                                text = "These weren't nearly as comfortable as my last pair of Sony 's and I can't get the foam earpiece back in after it came off. Really flimsy as I can't get it back on. The sound quality was the best feature and I'd give that a 4. But not worth the cheap quality at all.";
                                break;
                            case 9:
                                title = "Absolute rubbish";
                                text = "Maybe I got a bad pair. Hooked them into my sound board and then keyboard direct and were totally \"tinny\". Sounded more like something for $9 not $80. Totally awful. Plugged an old pair of Koss in and sound fantastic. Will be returning these. I am completely about quality sound and these ain't it.";
                                break;
                            case 10:
                                title = "Fool me once...";
                                text = "Fool me once shame on you, fool me twice shame on me. I bought a pair in 2013 that worked great but after a couple months of medium use the one side stopped working. I tried to deal with Sony who just told me to go buy a new pair. I was put off my the lack of support for something that seemed like a defect but I really did like the headphones so I bought a 2nd pair only to have the same thing happen after maybe 4 light uses. As much as I love the sound and feel of these head phones, with the multiple failures and total lack of support from Sony I will not be recommending these to anyone.";
                                break;
                            case 11:
                                title = "it used to work great but now the cable has failed";
                                text = "it used to work great but now the cable has failed, no longer to hear anything. Beware cable stops working after a while.";
                                break;
                            case 12:
                                title = "I heard these were like professional quality but i found them to be way ...";
                                text = "I heard these were like professional quality but i found them to be way less then that. I got a 20 dollar set of Behringer headphones that sound much better. These have a tinny sound to them, as well as when listening with them, i hear what sounds like an old 33 1/3 record scraping sound through them. I do like the heavy duty cord but thats not worth a thing if the sound quality is jacked.";
                                break;
                            case 13:
                                title = "They produced good sound but only for three months";
                                text = "I purchased these headphones and within 3 months they didn't work. When I looked into returning them the \"return window\" had closed. They produced good sound but only for three months.";
                                break;
                            case 14:
                                title = "No Longer than 7 months and the product stopped working";
                                text = "7 months in and all of a sudden, the product stopped working. First the instrument line stopped picking up my guitar, then when I tried a different USB port, the product stopped working entirely, will not turn on, even when I go back to the original port. It has been sitting on my desk, getting regular use, never moved, and now it's worthless. What the hell happened here?";
                                break;
                            case 15:
                                title = "The headphones worked great at first";
                                text = "The headphones worked great at first. Sound was clear, present, and what I expected. It hasn't been a year and they are already popping/cracking when sound is played. TERRIBLE for such a reputable company and model. For reference, I DO NOT play sound to any unreasonable capacity. If this is their quality of product and they won't replace it, they deserve to be in the $20 price range.";
                                break;
                            case 16:
                                title = "These are not over-ear. They are on-the-ear and are uncomfortable as can be - poor design.";
                                text = "Funny how Sony calls these \"over - ear\" when they are obviously not. They sit on top of your ears and hurt after a bit of wearing. The sound is fine but they are very small and uncomfortable. Plus the weight of the coiled cord is much more that modern straight cables and it also takes its toll from a comfort perspective. I wouldn't recommend these to anyone who valued comfort in headphones as a priority. Poor design in comfort and wearability.";
                                break;
                            case 17:
                                title = "So disappointed.";
                                text = "My Sony MDR-V150 set crapped out and these had great reviews so I decided to upgrade. Maybe this is a bad set but the bass is nonexistent on these. The noise canceling is great, I wore them while walking and couldn't hear my footfall. But the intro to the game I am playing has awesome bass that my previous (cheaper) set picked up beautifully and this set didn't register it at all. I am returning them and have ordered another MDR-V150 to be delivered ASAP.";
                                break;
                            case 18:
                                title = "Known defect, well documented online - Wish I knew before I wasted my time with these.";
                                text = "Every time I move my head or jaw these headphones produce a horrible sound - It's plastic creaking and crunching. It's unbearable. This is my second pair too - the first pair I returned because there was a screw rattling around inside.";
                                break;
                            case 19:
                                title = "DO NOT BUY A MDR7506 SAVE YOUR MONEY!!";
                                text = "I have been waiting for the opportunity to voice my opinion. First, the sound is great and that is about all. I purchased four (4) of these units because the sound is good. The mechanical features of this unit is extremely bad and really sucks. Wires break very easily around headpiece, outside covers of head piece fall apart by pealing. Black covering all over my ear and elsewhere. Sent units to Sony for repair and they said that they could not repair unit. They returned unit unrepaired and stretched the coiled wire beyond recognition. Second unit, same problem, third unit threw away from a high building, Fourth unit, hope, hope, hope, hope it stays together for awhile. Foolish for buying four units, yes, you are absolutely correct. I must be out of my mind for being such a sucker!...Never, Never, Never again will I purchase a thing from Sony. This segment of the company is truly VERY POOR and sucks badly....";
                                break;
                        }
                        break;
                    case 2:
                        switch (random.Next(0, 20))
                        {
                            case 0:
                                title = "Disappointed in the game play";
                                text = "Great storyline that was engaging to start. But once the game play actually got going, it was incredibly linear. You're trying to escape from guys at night but are forced to go through the trench and then the only building that they happen to be occupying?? In a world with so many sandbox like game engines, I just could not get into such a linear game. It was too bad as the story sounded very interesting, but not enough for me to sink hours of annoying game play into.";
                                break;
                            case 1:
                                title = "Great game, but a lazy, rather poor PS3 port that's still way overpriced for what it is.";
                                text = "Between two consoles PS3 and PS4 I've played this games story through a half a dozen times now and expect to play it through a few more. It's one of those games that has such a great story that you won't to pull it out and replay about once every year or two, like watching a favorite movie. So, I hate to give the game a bad review, but this seems like a really obvious cash grab to me. If you never bought it for the PS3, the PS4 is a no-brainer. But for those of us who already invested in the game once, Naughty Dog doesn't seem to have done nearly enough to warrant picking it up again, especially considering the high price this game still seems to command - as of this writing, the lowest it's dropped has been 29.99 for a real tangible game you can actually own and resell to help buy something else when you're through with it, the next console comes out, or if you don't like it as much as the marketing led you to believe. But being that this is supposed to be the definitive edition of the game, $30 seemed easier to swallow, figuring I already knew I loved the game enough to keep and there wouldn't be a more desirable (and cheaper) Game of the Year edition around the corner, even though $30 is still $10 more than many genuine PS4 games released around the same time have already dropped to - again, I don't count digital download code prices, because those are a complete waste of money IMO, unless the equivalent of a rental fee. Obviously the game is going to load faster on the PS4, though it wasn't at all problematic in that respect on the PS3. Typical of every PS4 game I've played so far, graphics being 1080p, projected at 1080p are more stable, with less scaling artifacts. Though some artifacting still remains, so I suspect the graphics were not actually rendered at 1080p, but I really don't know enough about the particulars of game rendering to diagnose further. As far as physical clarity/visible detail, I'm not aware of any improvement, from memory. Maybe seeing them side by side would illuminate something. But it's still not quite on the level of non-remastered PS4 games. And aside from the increased image stability previously mentioned, I'm hard pressed to note a tangible difference from memory, even after playing the PS3 edition through several times. What is especially disappointing, and to further illustrate the seemingly complete lack of effort that went into this \"Remastered Edition\", all of the level encoding issues that the PS3 edition had are still present and accounted for, even where Ellie goes out the garage door, just before her horseback subterfuge through the mountain village. I personally think the levels are off in the game in general, but some sequences are a little rediculously unpolished. Another pet peave of mine is the inconsistency of ammo pickups, which almost seem bugged. During the last segment of the power plant shootout, I'd gathered a total of 9 rounds off dead enemies, took a stray bullet, restarted the encounter to try and get through it clean, as on Survivor mode, every crumb the game gives you is precious, but the next time through, despite taking out each attacker the very same way I got no ammo pickups from them. None! Even the small caliber handgun that drops out of the guys hand in the bunk wasn't there and it's always been there in past runs via the PS3 edition. I restarted it three more times and still nothing, and I wouldn't find any more ammo for my nice new scoped magnum again until a scant few rounds at the very end of the game, making it pretty much useless. I haven't played the games multiplayer yet, as I don't have PS Plus. Assuming this is like every other PS4 game I've played, that may be another reason to keep the PS3 GOT edition - a multiplayer experience you don't have to pay more for, if buying new anyway. Ultimately, the game is worth it for the story alone. It's pros easily outweigh the cons, in regards to the game itself. But I can't help feeling a bit suckered into buying it twice, even though I'm the type who always repurchases the Game of the Year editions of games I can't get enough of.";
                                break;
                            case 2:
                                title = "Inconsistent enemy aggro mechanic can be quite annoying for the stealth bits";
                                text = "I expected the game to be a movie, but there's really not much game. Inconsistent enemy aggro mechanic can be quite annoying for the stealth bits.";
                                break;
                            case 3:
                                title = "Glitchy";
                                text = "I've played through TLOU three times on the PS3 so I know what to expect. When I upgraded to the PS4, this was the first game I bought. Started it a couple of days ago and so far it's super glitchy! Characters jump, disappear and reappear, and freeze. What's going on? Looks good, but not that much better than the PS3 version.";
                                break;
                            case 4:
                                title = "Meh...";
                                text = "I just couldn't get into it, I played the first two chapters, it just wasn't for me. I love the Uncharted series, the mechanics just didn't work for me.";
                                break;
                            case 5:
                                title = "Two Stars";
                                text = "story started off good then got weak. Controls don't work like they should overall not pleased with this game.";
                                break;
                            case 6:
                                title = "Tedious";
                                text = "Not the worst game I've played but most overrated... Since when did finding ladders become fun???";
                                break;
                            case 7:
                                title = "It ran great and looked beautiful";
                                text = "I know that I am in the minority here and that this game is universally beloved but it just doesn't do it for me. I didn't make it very far into the campaign because I could not stand the game play. I don't typically play survival style games with sparse resources and as soon as I started having to go hand to hand with the creatures and repeatedly dying I bailed. It ran great and looked beautiful, but this game just wasn't for me.";
                                break;
                            case 8:
                                title = "Not about the Gameplay";
                                text = "This game is all about production value, and not about gameplay at all. The gameplay comes down to classic, somewhat buggy stealth shooter elements and a slow, frustrating upgrade system. I was really excited to play this game as I had missed it on PS3 (being an xbox owner at the time), and really wanted to like it, but I really don't think this game is worth the buy. I truly do not understand the 5 star and 10/10 reviews this game received. I probably won't be finishing it, and that's a shame.";
                                break;
                            case 9:
                                title = "Enjoy it while you can. Sequel looks like they've lost their way!";
                                text = "THIS IS A REVIEW OF THE SEQUEL MORE THAN OF THIS ORIGINAL TLOU GAME. Mixture of shooter-type gaming and an apocalyptical story line. The 2 protagonists are fearless and funny, continually cracking jokes while at the edge of their demise. Think James Bond - they know they are immortal, so may as well have fun killing the bad guys. Terrific scenery and music. I liked it well enough to play it half a dozen times over the past few years. I'm glad I did, as I was tempted to look for a sequel. There are sequel clips available. Same main characters, but 5 years older. Those clips make the sequel look dark, dreary , depressing. No more cute Ellie - now she's a killing machine. No more mentor Joel, now he's no better at mayhem than the 5 years later Ellie. Throw in that the grown up Ellie turns out to be homosexual and I've saved 60 bucks! I'll keep my feelings about LGBT to myself, please keep it out of your games. Watching the few minutes of the Trailer was depressing.I can only imagine a game built on the re-purposed main characters. I can do a Google on \"Zombie Apocalypse games\" and get about a jillion hits. To repeat myself, I am satisfied that TLOU was one of the most thoughtful games I've played (I go back to the Atari days!). The sequel looks like pure garbage.";
                                break;
                            case 10:
                                title = "Good if you like artsy stuff that is 3edgy5you";
                                text = "13 hours of gameplay. No real replay value. Good if you like artsy stuff that is 3edgy5you. Great if you like a compelling narrative. I played this game in one 13 hour sitting. It was a great 13 hour stretch. My enjoyment would have been dramatically diminished if I had not played it all in one sitting.";
                                break;
                            case 11:
                                title = "Not as Advertised";
                                text = "This is a review for the seller. If this isn't the right way to do this then please ignore. Product is a coupon/voucher not the game itself and is not as advertised.";
                                break;
                            case 12:
                                title = "would make the game more personal and suck you into the story";
                                text = "I don't know where, but some information I read gave me the impression that this game was about a father and daughter who are left alive after a holocaust, and in the ruined United States attempt to make their way to the west coast together. Possibilities for all sorts of events where you know the father is totally invested in his daughter, would make the game more personal and suck you into the story. But, first, the daughter is killed off right at the beginning, which to me is a no-no! The movies think hard before they kill off an innocent who is a main character and you have gotten to know a bit. And if what I understood is true, I've been ripped off. I got the game because of my expectations of a father/daughter relationship. I guess I'm just not into zombies and ghouls. Also, the guy who lost his daughter, comes across a small girl later who looks to be the same age as his daughter was. I would think he would jump at the chance to be a father to her. Instead, he is a cold-blooded $***head who just wants her gone. I am not enjoying this game.";
                                break;
                            case 13:
                                title = "VERY Disappointing!";
                                text = "I played this game for about a half an hour after I picked up Ellie and just couldn't go on anymore. I kept hoping it would get better; I had such high hopes for this game because of all of positive reviews. I feel incredibly let down. This wasn't remotely scary and definitely not immersive. You are forced to follow a path and 95% of your enemies are humans. There isn't much to the crafting of weapons that I could see (though I did only get to the point where I could craft shivs). This does not feel like a post-apocalypse world AT ALL. Honestly, it felt like playing Call of Duty without the ability to run around and explore wherever you wanted to go. I found myself thinking that the gameplay was only starting out this way so you could learn how to play it; a tutorial start, so to speak. But no, the game really just keeps on like that. I cannot even fathom what everybody saw in this game that I missed. If you want a scary and immersive game where you feel like you are actually partaking in a post-apocalyptic world stick to Fallout 3 or Fallout New Vegas (I haven't played the other ones so I can't attest to their ability to satisfy). The takeaway: skip this game!! Or go in to it knowing it's absolutely nothing like what the reviewers claim it is. I wonder if Naughty Dog just hired a bunch of people to write stellar reviews?";
                                break;
                            case 14:
                                title = "Most overrated game in Playstation history";
                                text = "Hordes of bug-eyed fanboys speak is hushed, reverent tones about this game. And for the life of me I don't know why. It isn't a bad game, by any means. But the GOAT? Hardly. There is one great act, the Winter act. Other than that, it is hardly a game that distinguishes itself. In fact, I found myself fairly bored for stretches.And the voice acting had me cringing in places, it is so melodramatic and sappy. I gave it two stars because I really think it is the most overrated game in Playstation history. Fanboys who disagree need to start thinking for THEMSELVES and not mindlessly echo what their best buddies told them.";
                                break;
                            case 15:
                                title = "Boring sneak story mode game";
                                text = "I don't get all the great reviews. The game is just plain boring. I'm about 4 hours in and I'm gonna quit. It has a good story and nice graphics but basically all you do is run behind another character, sneak around zombies and watch tons of cut scenes that you can't skip. In this genre, Dying Light was much more fun for me.";
                                break;
                            case 16:
                                title = "This game just isn't as good as I expected";
                                text = "This game just isn't as good as I expected. The game play feels a lot like uncharted and so many of the 'puzzles' are just walk around until the find the thing you can move to stand on. And the thing you need to move to stand on just blends in to the rest of the dark background.";
                                break;
                            case 17:
                                title = "Most overrated game I've played.";
                                text = "This might just be the most overrated game of all time. I'll start off with saying the story is great although easily predictable. However; the gameplay is so generic with such a lack of diversity. It feels like one really long drawn out mission. The entire game. There's maybe 4 different enemy types in the game. Which they all look alike. The friendly AI is annoying how they get the way and take seconds to move. It's a great disappointment for me.";
                                break;
                            case 18:
                                title = "... filled with 4 letter words it would be a great game. It is bad enough when adults can ...";
                                text = "If this was not filled with 4 letter words it would be a great game. It is bad enough when adults can not seem to express themselves without using 4 letter words. But, to have a little girl talk like this is beyond bad taste. If a young person (I'm sure their a lot) were to play this game. They will soon be exposed to this as they way it should be. They may hear it from the uneducated and ignore, but to have a young girl in a video game talk like this is just in bad taste. There several issue that do not add up. She is born 6 years after the world has crashed. But, can drive a manual transmission truck for example. I only got part way through the game before I quit. Have seen bit and pieces of it on YouTube but not much change. It seem the many of YouTube channel seem to talk almost as dirty as this little girl does. If one can not express themselves without allow filth to come out. They should to learn to keep their mouth shut. It shows their ignorance of the English language.";
                                break;
                            case 19:
                                title = "I've got a friend who loved this game";
                                text = "Not a big fan of this game. I've got a friend who loved this game, I didn't care for it.";
                                break;
                        }
                        break;
                    case 3:
                        switch (random.Next(0, 20))
                        {
                            case 0:
                                title = "Clean design, AC power only, no throne, comes with sticks, easy setup, lots of electronic options for sound";
                                text = "I learned to play on a traditional drum kit, but because of where I live now, I don't think my neighbors would appreciate my skills, haha...this is a decent alternative. Set up was pretty easy (no language required) and the cables are clearly labeled and easy. If you can set up a DVD player...this is easier than that. The allen key comes with it and a small t-handle to tighten the kickdrum mallet to the pedal. In about two hours, I set up the stands, adjusted the pads to my level, tightened everything, and started playing. The heads aren't as big as I am used to...they are all roughly the same size. I'm used to a big snare head, 18\" floor tom, etc etc....but once you start playing, it just takes time to adjust to that.I plugged it into a Hartke HD 50 Bass Amp and it is more than enough sound for my place. It comes with a set of 5A sticks, you won't need more cause they won't get beat up.The electronic module only runs on AC power and you will need a 1 / 4\" input/output cable (or get adapters for the headphone-sized cable) if you want to use an amp. There is a headphone plug on the module, just make sure you have one with a long enough cord to reach your head. Also, you will need a throne to sit on and a piece of carpet to keep the kick pedal and hi-hat pedal rom sliding all over the place. Overall, it's a solid three stars from me. It would be four if I wasn't missing a screw & nut (it's not that serious) and it were actually an analog set. Well worth the price.";
                                break;
                            case 1:
                                title = "Many reviews I've read are correct";
                                text = "I just set this up and will update my review as I get more familiar but a couple of immediate reactions:\n- assembly instructions not great especially if you're left handed. need to do everything in reverse.\n- parts are given a letter but not labeled on the part. have to lay them out and go \"oh this looks like A...etc\". Not like I couldn't get it done, just an annoyance\n- another lefty comment, awkward to use headphones since the jack is to the left of the control module. Have to wrap around my back. Tried smaller/lighter phones but could hear the sticks hitting the pads\n- As many people have noted, heads are small and the hihat control is taking me a lot to get used to.\n- my last and most critical comment is that running my music through the aux in to play along just sounds horrible. If i turn my music up, the kit fades to barely audible. I don't just want to sit and play the kit to nothing. If I can't get happy with being able to play along comfortably, I will probably return the product I know this is a slightly premature review(and i will update it) but I'm looking for any feedback as to whether you think I'm right or wrong or have any suggestions. thanks";
                                break;
                            case 2:
                                title = "Worst building instructions ever";
                                text = "The product is not bad for a beginner's drum set. We shall see how long it lasts as my daughter is playing every day so far. But the disaster is, putting the thing together. Can the manufacturer's not put a decent set of instructions together with these things? Pieces corresponding with step by step instructions. It's a 4 hour build with these terrible directions.";
                                break;
                            case 3:
                                title = "Great kit for the price, but snare mesh already tearing.";
                                text = "I've had this kit 3 weeks, and so far it's great except for one issue: the snare mesh has already started to tear (pictured). I don't play particularly heavy, and if I end up having to buy a new snare every couple months, it's going to get awfully expensive. My other issue, and this is likely just a user ignorance issue, is that when I listen through headphones, I only get sound on one side. I may need to get a jack - splitter to get stereo from the headphone input...? Everything else about the kit has been great. Lots of features to learn still(obviously), but so far it's been a perfect solution for relatively quiet in-house practice. And I love being able to plug in an iPod/phone and play along.";
                                break;
                            case 4:
                                title = "You get what you pay for.";
                                text = "Ok drum set. The instructions were confusing, and was vague. It was a bit smaller than advertised, but a good thing because I dont have alot of room. The mesh heads feel great, and has alot of drum options. I just wish it said what drum kit you were using, in stead of numbers. all in all a ok set.You cant beat the price.I turned mine into a 3 piece set. Its much more simple to play now.";
                                break;
                            case 5:
                                title = "Impressive set besides hardware for kick drum pedal";
                                text = "Very impressed when I opened the set, but upon assembling it the clamp to hold the kick pedal on the kick drum would not tighten do to crack in clamp. Now I’m trying to enjoy this without a kick drum:(";
                                break;
                            case 6:
                                title = "Good kit for the price. Poor hi hat control.";
                                text = "Great kit for the price. But the hi hat only has 3 settings....maybe 4 Open, closed, and Sort of open. It is very frustrating playing jazz and funk styles..... you cant really play the hi hat with any subtleties, or even close it smoothly. It sounds VERY digital compared to the rest of the kit...I am hoping we can update this, I would buy a new pedal if so....";
                                break;
                            case 7:
                                title = "Low volume and need to buy an amplifier doesn’t work with aux cord";
                                text = "The volume is extremely low I was under the impression with just an aux chord I could hook it up to the stereo and play but the volume is so low you can’t hear it. And yes I turned every setting up to max. Apparently you have to buy an amplifier which is too much to deal with The module is super confusing";
                                break;
                            case 8:
                                title = "Too small";
                                text = "The only things that’s nice to me is the pedal and the adjustable drum heads. Of course it has a metronome and all that other stuff, but for an acoustic player, it’s just too small. The snare head is just too small Compared to the 11 inch snare drum I’m used too. The rims are way big, so unless you angle them just right, you’ll be hitting those too. The frame of the drum set is also kinda small, and that reduces the overall height of the whole set. Now I see people with this drum set that have a better set up, but getting more frame material is just going to cost for money. Everything else is okey but these problems overall just make playing this kit not enjoyable.";
                                break;
                            case 9:
                                title = "it arrived sealed but in the wrong dvd case and ...";
                                text = "it arrived sealed but in the wrong dvd case and all but one of the retention hubs were broken, all the disks were scratched but played, box it arrived in was undamaged but makes me wonder what is going on at the wherhouse";
                                break;
                            case 10:
                                title = "Jack...where are you";
                                text = "Miss the original cast. Still entertaining, but not the same";
                                break;
                            case 11:
                                title = "With SG-1 gone \"SYFY\" has really slipped.";
                                text = "I won't write you a \"book\" on this one, others have already done that. This was one well done series, in just about any aspect. Well written, it even \"looked\" good- unlike the typical SYFY Saturday movie. I always wondered why religion came into the plot line the last couple of seasons, negatively portrayed at that, but maybe the writers were running out of material. Well written even if you don't like the subject matter sometimes, or the enemy ( I \"liked\" the Goa'uld- seasons 1-8, better than the Ori- 9-10, and even then the stories had something to them. Start at the beginning of the series, not the end, and always be sure to watch the extras each season. This was the best of the three Stargate series.";
                                break;
                            case 12:
                                title = "Not original. 2nd hand copies.";
                                text = "There were \"Definitely\" missing episodes from the DVD's.";
                                break;
                            case 13:
                                title = "A Tired But Valiant Effort";
                                text = "Anyone who has tracked the adventures of SG-1 through its decade-long run will be fond of the team and its members. For me, the greatest pleasure in this ultimate season is watching the veteran cast members run through their paces with a healthy amount of self-deprecating humor. This irony reaches its high-point in the 200th episode, \"200.\" It rewards watching and re-watching just to catch all the \"inside\" sci-fi jokes. And it is also a pleasure to see Richard Dean Anderson back in the acting saddle again. On the other hand, there is a certain amount of listlessness in the performances; understandable for actors who know their show has been cancelled. Also, some of the conflicts seem strained and overly-contrived, as if the writers and producers felt it necessary to up the imaginative stakes but lacked the resources to back the effort. It was also disappointing to find the season lacked a cleaner resolution. No doubt the franchise bean-counters were thinking, \"thar's still gold in them thar Ori.\" We'll see if the anticipated direct-to-dvd movies prove them right.";
                                break;
                            case 14:
                                title = "Not as good as the others but still fun to watch";
                                text = "The overall series is a lot of fun to watch and some of this final season was still pretty good, however I didn't like how they did the final episodes. Several had nothing to do with the main plot and were just filler episodes. That close to the end they should have focused on the plot. It is still a good series and if you are watching it, watch it straight through from season 1 - 10 and don't forget the 2 final movies that finish everything off.";
                                break;
                            case 15:
                                title = "The end of a great show";
                                text = "It's a pity they ended this show as a bad video clip. And the whole season was boring, except for '200', The Shroud and The Quest I & II... Wish the writers had remained writing SG-1, not the Daniel/Vala show... and I'm a Daniel Jackson fan...";
                                break;
                            case 16:
                                title = "the end is near...";
                                text = "I am HUGE SG-1 fan but thought this season bombed,especially the finale. I would use a more colorful word but won't. Waiting to tie things up in a few later movies is a cop-out. I am glad to see this show end, given the way they handled the 10th season. Good-bye!";
                                break;
                            case 17:
                                title = "It was time";
                                text = "It was a great series since it's early days on showtime. Some plots were becoming repetitive. Even with new characters it was time. Of this last season I really only like 2-3 episodes. But if you have the entire series already, it's the completion.";
                                break;
                            case 18:
                                title = "sad to see stargate lost so much ground";
                                text = "sad to see stargate lost so much ground. it could have been lots better if the regular actors weren't going in so many directions. Of course, all of farscape actors didn't have to join up either..just a stargate farscape...nothing interesting there.";
                                break;
                            case 19:
                                title = "Worst of all the seasons and very dissapointing";
                                text = "I've always been a big Stargate fan. But I have to honestly say that season ten was their worst. It was like they just gave up and coasted through without finishing off the Ori or getting closure with so much. The last episode was terrible, lame, and stupid. They all just got old and then, oh wow, they got away from the Ori ship. Failed to finish the story line with Vala's daughter, Baul, the Asgard, heck with most of it. Season Ten in two words.... it sucked.";
                                break;
                        }
                        break;
                    case 4:
                        switch (random.Next(0, 20))
                        {
                            case 0:
                                title = "NIght sight pics are AWESOME";
                                text = "I love the size and shape of the phone as well as the weight. It fits well in my hands and was intuitive and easy to set up. I use my phone for just about everything these days and unfortunately, the 3a battery life leaves much to be expected. I basically end up leaving the battery saver function on all day in order to try and save battery. I love all other fuctions, the fingerprint scan works well, the volume is good enough, the screen is beautiful and the camera is AWESOME. I posted a pic of the full moon I took on the beach. Night Sight function made it look so clear its like a daytime shot of the sun! In my opinion the phone is worht the price thus far and im enjoying using it!";
                                break;
                            case 1:
                                title = "So far so good - but I wonder for how long";
                                text = "My old Pixel1 died - or rather the battery did, and well you cannot replace it without spending \"half a phone\" on it. I don't think any phone is worth $1000+ so I was happy that Google offered the 3a which has plenty of capacity for what I use my phone for for a reasonable price. I went from having 15-30 minutes of phone time on a full charge to days.\n\nThe process of copying data over failed because the old phone could not keep a charge long enough for the process to complete. With the help of Google support, I did a phone restore from the cloud instead, but this does not include _all_ data/apps. So the process of migrating from old to new phone, even within Google's own systems, was far from optimal and successful. Google support was very helpful of at least getting an operational phone with 85-90% of all features installed.\n\nYou just need patience - every app have to be downloaded, you have to re-authorize/authenticate everything - and if you like me use apps for OTP, you'll have to create all new keys.\n\nFeature wise I like the XL size better then the original size. The \"press for google assistant\" sucks and will soon be disabled. I love the finger-print authentication, I used that a lot on Pixel1 too and it's still easy to use. Camera works, even thought he google app for taking photos is getting worse.\n\nThe driving-mode feature sorta worked on the old Pixel, but it's like it has all new life on Pixel3a. The only problem is that it's over aggressive. Sitting on a plane I had to constantly turn it off as we taxied to take off (airplane mode does not disable GPS). But in my car it's GREAT - I start playing podcasts, set my destination and as I drive out the garage the drive-app activates and now incoming calls are \"restricted\" and full hands-free works great. The old pixel, this was all a manual process - and it would get confused at times and refuse to stop podcasts when calls came in.\n\nGiven my experience with the pixel1 I'm unsure of the longevity of this phone. It has the same design fault not giving you access to the battery without a lot of special tools or spending quite a lot of money to have someone else do it. The next battery failure will be the last one for Pixel - at least for me.";
                                break;
                            case 2:
                                title = "This is a solid, reliable phone/device";
                                text = "I have only had android devices and always went on the cheap side. This one works markedly better and has great battery life while also charging fast. I don't use it much, but every aspect of it seems top notch. The camera is versatile and very easy to use.";
                                break;
                            case 3:
                                title = "Purple-ish?";
                                text = "Ordered the Purple-ish phone. The phone came, was easy to set up and synch with the old phone, and worked great with my T-Mobile sim-card. Seems to work fine, but the phone that came was white. I's OK, and I have a protective color on the back, but it seemed weird that I did not actually get what I ordered.";
                                break;
                            case 4:
                                title = "Great phone, but Amazon let me down this time.";
                                text = "The phone itself is 5/5 stars, but Amazon's service was not so good.\n\nThe phone was opened on arrival (eg. seals were broken). Fortunately, the contents was intact. Shipping took two more days than expected because UPS sucks and refused to make a second delivery attempt.\n\nAs for the Pixel itself, it is a great phone. I love the fact that it will have the latest android every time and it has a headphone jack! The plastic back doesn't feel cheap or flimsy. Besides, 99% of people use a case anyway. Having a glass or aluminum back is pointless.\n\nOverall great phone, this is no nexus but it's up there.";
                                break;
                            case 5:
                                title = "This Pixel is outstanding..GREAT PRICE ..easy setup..easy transfer of contacts from another device";
                                text = "This Pixel is OUTSTANDING.....PRICE IS OUTSTANDING...GOOGLE FI CELLULAR is best pricing, with features such as overseas coverage, I could find. Support is so important and support for any issues are quickly addressed thru warranty, chat, repair centers, and even a real person If needed...a real person you can understand perfectly.";
                                break;
                            case 6:
                                title = "Robust , neat and stylish";
                                text = "First of all to answer any concerns about this , the camera is great! Very high quality photos and pictures taken at night seems to be taken by a professional camera man in a studio . What is not good about the camera : Does not support manual mode. Mobile is fast can handle many tasks processor is great RAM is also working fine , it heats up a little(to a warm degree similar to holding a coffee cup using a collar ) after 3 + hours of continuous use but i am not judging this as a bad side .\nThe package of the mobile itself is regular nothing too fancy it does contain however a groovy Google logo sticker (I didn't place it on the mobile though . The back side of the mobile feels so good to hold that i kinda miss it due to the protective case i am using. Selfie cam is awesome if i haven't mentioned that yet.\nThe small things I don't like are basically some software issues that will hopefully be solved in the near future , for example most android mobiles has the ability to flip the phone while receiving a call to mute the ring tone ; that is missing! My old Asus phone had a cool feature which is the tap tap(knock knock as i liked to call) which is double tap on the screen unlocks the mobile or prompts you with the pattern . In the pixel 3a one must either press the lock button or use the finger print sensor which is very efficient and have worked perfectly even if my finger was not exactly clean or a little bit wet.\nThe phone however asks for patter reconfirmation a lot I don't know why? Another thing would be if you are an individual concerned about your privacy this phone will protect your privacy from any app you want but google apps , using the google assistant to ask for directions to the airport would work fine but when you click on the map it came up with for the fastest route the phone will ask you for location service permission , the phone consumes background data and uses any service in the mobile for google's favor without any permission what so ever but for other apps they are pretty careful about what YOU share .\nDefinitely a good deal for the price and you are having a phone that looks like a high end product although it is a middle class phone and it also has a middle class price which shocks people about the great bargain you have ! ;)";
                                break;
                            case 7:
                                title = "Great phone!";
                                text = "My son upgraded to this phone and so far he really likes the capabilities and functionality. Great phone for the \"budget\" price.";
                                break;
                            case 8:
                                title = "Pixel but 1/2 price";
                                text = "Great bang for buck";
                                break;
                            case 9:
                                title = "Nice Phone";
                                text = "Seems to be working good, very nice camera would have given 5 stars if it had a microSD slot";
                                break;
                            case 10:
                                title = "Battery drains faster than expected";
                                text = "Pros: camera, price, good performance\nCons: battery drains too fast; faster than my 3 and a half years old iPhone 6s Plus";
                                break;
                            case 11:
                                title = "No earbuds? Really";
                                text = "My wife and I loved our new pixel 3a but...how is it possible there are no earbuds in the box? Life without music does not make sense at all: (";
                                break;
                            case 12:
                                title = "Great camera, mid range performance";
                                text = "Great camera great overall, but the slower processor struggles at times.";
                                break;
                            case 13:
                                title = "Overall a good phone!";
                                text = "Light Weight!";
                                break;
                            case 14:
                                title = "A breath of fresh air";
                                text = "The pixel 3a is everything the pixel should’ve been. A lower cost midranger with killer software that rivals the status quo. It tries hard to be Apple at times, for better and worse, and over the years Google has made improvements within their own ecosystem that truly makes this phone seem more intuitive than the average Android.\n\nWhat hasn’t been said about this phone, it’s a bargain, especially when it’s on sale. The camera is easily better than most flagships. Battery life is more than adequate, not phenomenal - you'll notice battery dropping on standby 1 - 2% per hour when not in use. Even with that, battery will last 6 - 7 hours on screen with a quick charge back to 100%, sometimes in under an hour with compatible accessories - another caveat is that tinkering with options such as turning wifi search off, restricting location access, and more found on Reddit will bring a more robust battery. Screen is crisp, makes colors pop and leaves nothing to be desired, adaptive lighting still needs work here. I was really blown away with how responsive this phone was, not to mention call quality is some of the best I've experienced and call over wifi is a great addition.\n\nDon’t read too much into the people who trash this phone for not having the latest gimmick - wireless charging, or the IP rating (why do people want to shower with their phones?), or even a full glass build because it accomplishes so much for what you pay. If you take a chance to look around right now there is no other smartphone on the market that offers so much in this price range, and if it is, there is typically a large sacrifice - poor camera, old displays, low specs, no updates. I would NOT recommend this phone to anyone who values privacy, does not like Android, heavily invested in iOS or likes the simplicity of iOS or likes small phones. I had trouble getting used to the large screen of the pixel 3a, even with gestures turned on in Q beta, it feels great in hand but adding a case makes it seem too bulky. Make no mistake - Google owns this phone, your information, your location, like it or not - and has a reputation for poor customer support. Just an FYI for anyone thinking this is best thing since sliced bread, it's an incredible value but still a a Google product (you may be a paying beta tester).\n\nBottom line: it’s not premium and it wasn’t designed to be. This is a great upgrade if you're coming from an older phone (Samsung s8 or earlier, iPhone 6 or earlier) and don't want to needlessly spend more money or a budget android - anything newer, you're better off skipping this phone entirely unless you really need that amazing camera. This is the phone that will serve you well and not make you self conscious about buying it, dropping it once every so often, or snapping pics at a party. At the end of the day a phone is a tool we use to experience and view the world around us, not an investment or status symbol, and this phone certainly gets it done. Finally, the market is starting to head in a new direction, and I welcome it if it’s anything like the Pixel 3a which is a huge breath of fresh air.";
                                break;
                            case 15:
                                title = "Decent phone but not outstanding";
                                text = "I bought my pixel 3a at a T-Mobile store, a d not on Amazon. I have had it now a few months, and think that I can now give a good opinion of how I feel about it. The sound quality is just decent. The speakers are not as loud as people make them sound to be. The fingerprint reader is decent. I'm coming from an honor 6X, which has just as fast a fingerprint reader as the 3a does. The quality of the screen is good. You can see it in sunlight, but it's not very clear. The battery life is just average. I don't play games on my phone, and compared to my honor 6X which I could go a full day, almost a day-and-a-half I barely get three-quarters of a day with the 3A. I was an AT&t prepaid user, now I'm with T-Mobile. Because of the phone I have Wi-Fi calling and voice over LTE. The reception has been pretty good in most places. I have not used to mobile hotspot yet, but I have the ability to do it using the T-Mobile plan that I have. It synchs up well with Bluetooth, and I have not had a problem with reception via Bluetooth. Would I buy this phone again?... Yes, but is it all that they say it is, I don't think so.";
                                break;
                            case 16:
                                title = "Had it 2 or 3 weeks, will keep updating this review.";
                                text = "Replaced a POS Moto G5 with this. Phone is nice, a little bigger than I like, but not big compared to most of today's phones. Had an interesting time transferring photos from my PC to this phone. I found that if I copied into an empty folder on the phone, it worked fine. If it ever gave me a message about skip or replace, regardless of the answer when it was done \"copying\" no new files were on the phone. Google assistant was not as easy as it should have been to disable and then the phone kept asking me to re-enable it. So I got rid of Pixel menu and Google assistant and went back to Evie. The hardware volume control is worthless as it only adjusts one of the volumes. For some reason the notification volume is linked to the ringer volume so I had to install a 3rd party app to separate those. The built in ringtones were too quiet for me so I downloaded a really loud one from Zedge. I love the built in call screener. Have not had much to do with the camera yet, they say its really good. I would rather the fingerprint scanner be on the front. Will write more later. Update 7/17 I like the phone. The speaker phone fidelity is terrible. Still haven't done much with the camera. (My previous POS phone got me out of the habit of taking pics). That's the only negative (the fidelity). All in all, I'll get another one of these for the wife. Update 7/28 the only two problems, one small one large. Small: whenever you adjust the ringer volume it has to play a sample. I turn the ringer volume down when I go to bed. Would like to turn it up when I awake but can't because it blasts out 3 seconds of the loud ringer and Don't want to wake others. Would like to be able to adjust the ringer slide without audio confirmation. Second problem, big, people that I call have trouble understanding me and me them.";
                                break;
                            case 17:
                                title = "A great phone...most of the time.";
                                text = "I bought my Google Pixel 3a in late May 2019, just before my road trip back to Chicagoland for the summer. It has a lot of great features, all which other more technically-minded reviewers have already noted. So I will keep this brief and list the pro and con only.\n\nPro:\nIt's lightweight and slender, so easy even for smaller hands to maneuver\nThe camera is excellent (especially for a budget price), with included photo software\nCharges via USB-C, making it a fast charger\nBattery good for six hours with moderate data use\nComes with some great apps already included\nSyncs well with my Chromebook\n\nCon:\nThe phone is buggy. It has butt-dialed several of my friends, much to my embarrassment. Once it even created a video chat with two people who don't even know each other.\nThe battery easily overheats when using my GPS app.\nThe battery is not removable\n\nI bought my phone via a well-known Photography store out of New York City. It came with a free three-month trial for a contract-less phone carrier that I ended up switching to.\n\nI am generally very happy with this Pixel. I've taken some great landscape shots with this phone that compare to my old Canon 5D. I use a Chromebook for most of my internet needs, and this syncs very well. But the buggy phone and the sensitive battery keep me from giving this five stars. It comes pretty darn close, especially since this is the best phone I've gotten for under USD 400.\n\nFor people who love Chromebooks and Google apps, this may be the phone of their dreams.";
                                break;
                            case 18:
                                title = "Great value!";
                                text = "Great phone! I transitioned from the iPhone and I'm so glad I made the switch. Great camera, battery, and user-friendly. You can't beat the price either! The value is amazing!";
                                break;
                            case 19:
                                title = "Dead pixel on the screen";
                                text = "Great camera. But I had dead pixel, so decided to return back and buy a new one.";
                                break;
                        }
                        break;
                    case 5:
                        switch (random.Next(0, 20))
                        {
                            case 0:
                                title = "Converted me to a PS4 fanboy";
                                text = "I never had a playstation 2 or 3 so I missed out on this game when it first came out. My roommate bought a PS4 so I thought I'd try this out considering all the praise it received. It looks fantastic, if I didn't know it was remastered I wouldn't have thought it was from a previous generation console. Story is well written and the animation and modeling on the characters is very realistic. The gameplay gets a little repetitive by the end but I really enjoyed having to make it through certain levels with only a brick and a shiv. The crafting system is just the right balance of making exploring the environments rewarding while also forcing you to conserve every bullet. I died a lot. If I had played this back in 2013 I would've thought it was one of the best games ever made. Playing it now isn't nearly as jaw-dropping but I can still appreciate how well-polished everything is. Now I'm really looking forward to the Last of Us II.";
                                break;
                            case 1:
                                title = "A True Marvel in Gaming";
                                text = "The Last of US Remastered is a remastered version of the PS3 hit the Last of Us. You follow Joel as he travels across The United States of America with his companion Ellie as she is the key to finding a potential cure for the disease that has wiped out 60% of the world's population. The Last of Us was a phenomenal game. Naughty Dog did not go lazy with this port to the PS4. In this version of the Last of Us you get full 1080p and also 60 frames per second. Some of the textures are better and the lighting is just over all better. This is one of the most cinematic games out their, but it never feels as if they take the game out of your control. Being movie like but still knowing it's a game is a rarity to find in most cinematic story driven games. This is the definitive version of the Last of Us which is worth a second buy even if you bought the PS3 version of the Last of Us. What makes this version so good is that you get everything in this. All the DLCs and the multiplayer maps. Plus the upgraded graphics although The Last of Us still looks good on PS3. The story is great Naughty Dog out did themselves with the story. What sells the story is the characters. Joel and Ellie make the game's story work. You see their fight against the post-apocalyptic world while developing a father daughter type of relationship. The games' prologue is a gut wrencher. Troy Baker and Ashley Johnson bring the characters to life. The Last of Us has some of the best voice acting in gaming. The gameplay keeps to the world as ammo and resources are limited. This makes for fun and tense combat, it makes the stealth in the game a useful play style as young waste less resources. Hand to hand is one the best in a non fighting game with you feeling every punch and crack. The gun play is also great because you must make every shot count. The game doesn't give you an abundance of ammo unless you put it on easy. The sounds and soundtrack are also worth a mention. Gustavo Sandoval's orginal soundtrack definitely fits the world and fits in its atmosphere. The sounds are also amazing as you hear the infected and their grueling sounds. Multiplayer returns in this remastered version. It's still the same old fun that the old version was but better. The 60 frames per second just helps the gameplay feel smoother. I really didn't like the Last of Us' multiplayer when I first played but playing on the PS4 made me scold myself for being stupid. The Last of Us is truly something special in a market oversaturated with countless of games, movies, and TV shows. The Last of Us still manages to maintain its own identity and refrain from being another cliché zombie title. Visiting it a second time I could see why the game won all the praise and awards it did. The Last of Us is still a masterpiece and I hope to see more of this world. The Last of Us is about 14-15 hour to complete and plus another 2 with the DLC story Left Behind. Well worth full price The Last of Us is truly a system seller.";
                                break;
                            case 2:
                                title = "A fantastic story like you've never played.";
                                text = "A powerful display of love and death, conflict and resolution, and the steps we take to protect the ones we care about the most.\n\nThe Last of Us gives you superior gameplay while also delivering a palpable story. However, if you are looking for a fast-paced shooter with lots of skull-cracking, you best keep looking.\n\nThe Last of Us will have your heart pounding with different emotions, whether that be endearment or suspenseful nervousness, and I am glad I got the chance to play such a great game. I love the Uncharted series, but this takes the cake as Naughty Dogs masterpiece. Not even Crash Bandicoot or Jak & Daxter could contest that, and not only this, but The Last of Us 2 is being developed, and I am sure it will be as good as this one.";
                                break;
                            case 3:
                                title = "Really engaging story with great characters and fluid game play";
                                text = "This is a top-tier game. I am a huge fan of zombie games and this one is really an exceptional experience. It is not of the Dying Light or Dead Rising style where hoards of zombies are always in your way, in fact you spend more time fighting other human survivors than zombies. But the characters and the story are really excellent, the visuals are stunning, and I find the game play mechanics fluid and pleasing to use. The premise of the weapons and tactics is based around realistic scarcity (of ammo, guns, everything) and that takes some getting used to if you come from other zombie games, but believe me it is worth it.";
                                break;
                            case 4:
                                title = "This is a good, fun game and nicely challenging";
                                text = "This is a good, fun game and nicely challenging. The characters are realistic-- they each have their own vulnerabilities and strengths and don't fall to stereotype easily. I'm picky about my games because I tend to have very little free time so I took a chance with this one and loved it. The combat system is good and the tactical movement, \"listen\" mode, and other fighting action is a good mix of realism and drama with playability. The \"zombie\" plot has a nice twist; in this case it isn't brain-chomping undead but people infected by a brain fungus that turns them hyper-violent, more like the first \"28 Days Later\" movie.\n\nThere's a boss fight in a burning ski-resort restaurant that is pretty intense and starts out as a great cat-and-mouse but in my opinion can get tedious towards the end --\"Can I just kill this guy and get going again? Please?\"-- but the rest of the combat action is great. The crafting part is a bit limiting: it seems you can only carry three of any one type of thing at a time, which doesn't make sense. Only three pieces of broken scissors before you reach \"peak broken scissors\"? But you do have to choose how to use your components carefully since the same components are used to make multiple different items, so you have to choose between using your alcohol to either make a molotov cocktail or a first aid kit. Once you make one, you can't dismantle it to make the other.\n\nThere's some DLC that has been added in the last few years, including a great side adventure that explains Ellie's motivations. It's an excellent side adventure that involves a lot of flashbacks. There's one weird scene, though, where Ellie \"playing\" an old arcade-style video game and the sequence of using the game is described by another character's narration, and you have to match the joystick movements to what the character describes for a game within a game. It seemed odd and out of place and got tedious really quick, so I just mashed a few buttons until it was done just to get going again. Otherwise the DLC was an excellent chapter.";
                                break;
                            case 5:
                                title = "Masterful Game, If you get one game for the PS4 make sure it's this one";
                                text = "A bit of warning, this game is definitely not made for the faint of heart. Has a ton of gore and gets pretty horrific at times. You're basically going through a horror movie, so I can see someone who might not enjoy that genre not enjoy this game. With that said, almost every area of this game is off the charts good. Graphics are the best I've ever seen for any game period; so realistic I don't know they could possibly top it in the future. Audio is well - crafted to and woven into the game flawlessly.Story is touching and suspenseful, moves at a steady pace never moves too slow or too fast. One of the few games I've played that really makes it fun to search every twist and turn. Combat is extremely fun and like many games coming out nowadays have, you're usually given either the stealth route or go guns a blazing. I must admit the game does tend to get a bit repetitive after a while, but the story takes so many unexpected twists and turns, I almost didn't mind it. This game is definitely worth experiencing, and if you were to only get one game for the PS4 I'd recommend this one in a heartbeat.";
                                break;
                            case 6:
                                title = "AAA Title";
                                text = "First off let me say I never played this game on PS3 , and yes I'm a sony fan . Just never got around to playing It was always busy with something else . Picked up my PS4 and battlefield 4 and wolfenstein after both of them woolahh they released the last of us remastered for PS4 instantly jumped on It been playing now for over a month . This Is where I'm going to disappoint the hardcore last of us fans I'm only a quarter Into the story but I just can't escape the online . It's very fun possibly one of the funnest multiplayers I've ever played . It's fun and quite challenging unlike other online shooters , Never really played a online 3rd person shooter and man Is It fun . I'm no hardcore gaming genius I'm not going to sit here and type five paragraphs only thing I can say Is this game Is very fun well worth the $50 the online Is amazing and honestly the little of the story I have played Is great , I can tell It's going to be a awesome story to the game , and that's what the last of us Is known for . Take the jump and order this game you will not be disappointed . This game will have a lot of replay value . Comes with the map packs , and the story dlc . Naughty dog didn't disappoint on there product but they do disappoint on trying to milk this game , always coming out with new mask , new weapons ,perks etc but theres a catch you have to pay . I broke down and bought a couple perks, and a couple mask , and a new pistol . Over all the mask wasn't worth It but the perks and the pistol was worth the money but should be included In the game ! That's all folks !!!";
                                break;
                            case 7:
                                title = "One of the best games for PS4";
                                text = "I played this game along with my kid for so many months. It has a strong story along with strong characters. The walkthrough of the game is quite enjoyable with many conversations between the characters as well as so many dangerous gangs or \"monsters\" to deal with. I really enjoyed this game and I'm waiting for the second part to be released later this year.";
                                break;
                            case 8:
                                title = "the best video game I have ever played";
                                text = "Simply, the best video game I have ever played. Story, visuals and gameplay are excellent. I've replayed this game four times. Purchased the remastered when I upgraded to current gen. Naughty Dog rocks. I gave Unchartered 4 a try because ND made it, and although not really my typical style, I really enjoyed that game. If you play video games even casually, you must play this title, it simply is fantastic and it really stays with you after completed. Has definite replay value and comes with an excellent DLC that enriches the main story. Highly recommended.";
                                break;
                            case 9:
                                title = "Great game/graphics & story telling.";
                                text = "I don't usually play the zombie/infected types of games they just don't appeal to me. I'm so glad I went with this one it was fantastic from start to finish. During game play you witness the connection of the two characters & the bond that starts to form between them. I'm not going to ruin any in game moment's. I will say the game is addicting, the story is well thaught out & pieced together & the graphics of the game set to 1080p are unbelievably fantastic. I really am looking forward to part 2. I also like the fact the DLC also came on the disc that shows ellie's story.";
                                break;
                            case 10:
                                title = "TLOUR was worth the purchase.";
                                text = "When I heard one of the best games last gen was getting remastered, I had to pre-order it. The game still looks great, it isn't a huge leap over the PS3 version visuals wise, but you can see the difference especially in the character models. One thing that makes the PS4 version better over the PS3 is the game being native 1080p 60fps. The 1080p helps the detail on a lot of things such as signs or books more crisp and clearer to read. With the frame - rate getting boosted up to 60fps, the games runs smoother.(On a side note its not a lock 60fps, so it will dip to a low 50fps when things get too drastic but most of the time it stays 60fps.) If you don't like 60fps Naughty Dog has put in a option for a lock 30fps which doesn't dip in anyway. The sound in this game was already excellent but Naughty Dog added new settings for the audio option to support a variety of audio formats similar to the Uncharted series has.\nThe story for the sp is by far one of the best stories I've played in a game. From the start to the end you won't be disappointed in it. The voice acting is top notch making the story more satisfying.\nThe mechanics of the game still the same not much has changed and the controls feel better with the DS4. The buttons are the same just with minor changes. L2 and R2 are now your aim and shoot button while L1 and R1 are your run and listen mode. Touch-pad brings up your backpack and audio recordings/flashlight sound will play through the DS4 speaker now. There is a option to turn the L2 and R2 back to the DS3 layout of L1 and R1. MP is the same but in 1080p and 60fps now making the online much smoother than the PS3 version was. Best feature added is photo - mode, similar to what Infamous SS has you will be able to stop time, use different settings to change the camera angle and add camera filters to take a picture of something in the game.\nOverall the game is a masterpiece and purchasing this if you didn't get a chance to play it on the PS3 will be worth it.";
                                break;
                            case 11:
                                title = "Another great game by Naughty Dog";
                                text = "I absolutely love this game. I have always loved zombie games but Naughty Dog has done it again. This blows Resident Evil out of the park. I have learned since the start of Uncharted that when you see the Naughty Dog logo you are getting a great story, great characters and an experience like none other. I initially bought this for a PS3 but have upgraded and was a little upset I was without this game. Once I saw the announcement of The Last of Us for the PS4 I was excited. The graphics are way better, the sound is way better and it has the Left Behind DLC included on the disc. The Last of Us shows us what life in a zombie apocolypse would be like. You would do what you needed to do to survive. I loved how when Joel first met Ellie, she was just a job. As the story progressed they form a relationship and he is super protective of her. The other thing that really made this game stand out to me is the facial expressions. When Joel is upset his face shows it. Great job Naughty Dog. Can't wait until Uncharted 4.";
                                break;
                            case 12:
                                title = "Excellent Horror survival game that mixes lots of feeling between ...";
                                text = "Excellent Horror survival game that mixes lots of feeling between personages that are part of the story. The beginning was not that involving story. Far from what is a zombies shooting game, this is more a love story between a father who lost his doughtier years before and Ellis a teen age girl alone that need find the need to have someone who love as father taking care of her. Interesting also see how this relation between the two of them develop step by step until to give risk their own life in order to rescue each other.\nI really love this game and the theme.";
                                break;
                            case 13:
                                title = "One of the best games ever.";
                                text = "One of the best games of all time with the best character development in gaming. Very nearly everything about it is great, from the gameplay to the voice acting to the characters to the world that's been created. With all of the hype that was surrounding it, I kept my expectations in check, but I really didn't need to. Even with high expectations, The Last of Us would've blown them out of the water. The journey that Joel and Ellie take, both individually and together, is one of the greatest things I've ever experienced in a game, or in any story. It's gritty, genuine, and serious but with the appropriate amount of humor and a slight amount of hope to make it endearing instead of too depressing. Everything just works. I played it three times in a row and never skipped a cutscene. The review I wrote elsewhere about this game ended up taking about two weeks and was around ten pages long just because of how much there is to say about The Last of Us, and all of the thoughts that it inspires.";
                                break;
                            case 14:
                                title = "It's fun.";
                                text = "One of the best games I played. You will love the every single minutes of this game. Just buy it and play! Was planning to stop playing around 2 am and sleep, but than when I finished this game from the half way, I realized outside was bright. But I was happy to go back to sleep at that time. Must be the reason why you would buy PS4, and I can't wait for the last of us 2 coming out this year or next year. Multiplayer was pretty fun too. Since this game is quite old, people are kind of too good in there, but it was pretty fun to play. It's only around $20, and I am extremely pleased with the game. It's totally worth your money.";
                                break;
                            case 15:
                                title = "One of my favorite games of all time";
                                text = "I am not normally into zombie games, but I decided to give this one a try on a whim a few years ago because the game was cheap. And it was completely awesome. The game play is great, and the story is awesome. The best story I have encountered in video games. I fell in love with the Last of Us and I have played it so many times since then and have most of the trophies. I have the downloaded version but got into speed running the game and that does best on the unpatched version and you can only get that with with the cd version.";
                                break;
                            case 16:
                                title = "Awesome game";
                                text = "I loved this game when it came out for the ps3. Enough to buy it for the PS4 when I spotted a deal. The optimization for the PS4 was very well executed. Had some minor bugs but nothing too serious. 80+ hrs with maybe 2-3 bugs that I noticed. If you haven't played it, what are you waiting for... Can't wait for TLOU 2 If my review helped you in any way, please let me know by clicking the \"helpful\" button below. Any questions you may have, feel free to ask… Thank you!";
                                break;
                            case 17:
                                title = "Better than a movie.";
                                text = "One of the most emotionally powerful games I've played. Excellent storyline, excellent acting and characters, and most important?--excellent gameplay. Definitely in my top 3 for the ps4. If you aren't a casual gamer and you want to play a game that is truly an epic experience akin to an amazing book or an incredible film (but is simultaneously an incredible action packed shooter/thriller) then play this game. You won't regret it.";
                                break;
                            case 18:
                                title = "Can't wait for TLoU2";
                                text = "I started off on hard mode and was really annoyed because I kept dying to those damn clickers. The game gets much better once you actually have other people to kill so stick around! Also, if you're going for achievements just play normal--not hard. You'll have to replay it anyway.";
                                break;
                            case 19:
                                title = "Wish I could play it again for the first time!";
                                text = "No lie, this is one of the greatest games I've ever played,...EVER! I've been playing since the Atari days and it's nice to still be able to be completely drawn into the story. This (for better or worse) is almost like playing a movie! For me, that's VERY good as I was totally engrossed in the story and relationship that was between Joel and Ellie. This is, in fact, probably the only game I've played through on the hardest difficulty because I loved it so much! Phenomenal game! Wish there would be a sequel (and there probably will be) but I don't think there's any way to one up the original!";
                                break;
                        }
                        break;
                }





                ProductReview review = new ProductReview
                {
                    Title = title,
                    Rating = rating,
                    Text = text,
                    IsVerified = random.Next(0, 2) == 1
                };

                // Assign the customer to the review
                review.CustomerId = customerId;

                review.ProductId = product.Id;

                // Add the new review
                unitOfWork.ProductReviews.Add(review);



                // Increment the star based on the rating. So if the rating is 3, the threeStars property will be incremented
                switch (review.Rating)
                {
                    case 1:
                        product.OneStar++;
                        break;

                    case 2:
                        product.TwoStars++;
                        break;

                    case 3:
                        product.ThreeStars++;
                        break;

                    case 4:
                        product.FourStars++;
                        break;

                    case 5:
                        product.FiveStars++;
                        break;
                }

                // Increment total reviews
                product.TotalReviews++;

                // Calculate the product's rating
                double sum = (5 * product.FiveStars) +
                             (4 * product.FourStars) +
                             (3 * product.ThreeStars) +
                             (2 * product.TwoStars) +
                             (1 * product.OneStar);

                product.Rating = Math.Round(sum / product.TotalReviews, 1);

                // Update the product and save the changes to the database
                unitOfWork.Products.Update(product);
            }


        }


        private void SetProductOrders(string customerId, List<Product> products)
        {
            Random random = new Random();

            int numOrders = random.Next(1, 21);


            for (int j = 0; j < numOrders; j++)
            {
                string orderId = Guid.NewGuid().ToString("N").Substring(0, 21).ToUpper();

                Product product = products[random.Next(0, products.Count)];

                List<OrderProduct> orderProducts = GetOrderProducts(orderId, product);


                double subtotal = 0;

                orderProducts.ForEach((OrderProduct orderProduct) =>
                {
                    subtotal += orderProduct.Price;
                });

                double shipping = Math.Round(random.NextDouble() * 5, 2);
                double discount = Math.Round(random.NextDouble() * 2, 2);
                double tax = Math.Round(random.NextDouble() * 3, 2);
                double total = subtotal + shipping - discount + tax;



                ProductOrder productOrder = new ProductOrder
                {
                    Id = orderId,
                    CustomerId = customerId,
                    Date = RandomDay(),
                    PaymentMethod = random.Next(0, 8),
                    Subtotal = subtotal,
                    ShippingHandling = shipping,
                    Discount = discount,
                    Tax = tax,
                    Total = total,
                    ProductId = product.Id,
                    OrderProducts = orderProducts
                };

                unitOfWork.ProductOrders.Add(productOrder);
            }




        }



        private async Task SetListProductsAsync(Guid collaboratorId, List<Product> products)
        {
            Random random = new Random();

            int numProducts = random.Next(1, 11);

            for (int j = 0; j < numProducts; j++)
            {
                string ProductId = products[random.Next(0, products.Count)].Id;
                if (!await unitOfWork.ListProducts.Any(x => x.ProductId == ProductId && x.CollaboratorId == collaboratorId))
                {
                    unitOfWork.ListProducts.Add(new ListProduct
                    {
                        ProductId = ProductId,
                        CollaboratorId = collaboratorId,
                        DateAdded = RandomDay()
                    });

                    await unitOfWork.Save();
                }
            }
        }


        private DateTime RandomDay()
        {
            Random gen = new Random();
            DateTime start = new DateTime(2016, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }


        private List<OrderProduct> GetOrderProducts(string orderId, Product product)
        {
            Random random = new Random();
            int numProducts = random.Next(1, 11);

            List<OrderProduct> products = new List<OrderProduct>();

            for (int i = 0; i < numProducts; i++)
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 25).ToUpper(),
                    OrderId = orderId,
                    Title = i == 0 ? product.Title : GetProductTitle(),
                    Type = random.Next(0, 3),
                    Quantity = random.Next(1, 3),
                    Price = i == 0 ? product.MinPrice : Math.Round(random.NextDouble() * 10, 2),
                    IsMain = i == 0 ? true : false
                };

                products.Add(orderProduct);
            }

            return products;

        }


        private string GetProductTitle()
        {
            Random random = new Random();
            int rndNum = random.Next(0, 21);
            string title = string.Empty;

            switch (rndNum)
            {
                case 0:
                    title = "Form Fitting Gloves";
                    break;
                case 1:
                    title = "The Amazing Flashlight";
                    break;
                case 2:
                    title = "The Last of Us";
                    break;
                case 3:
                    title = "The Last of Us 2";
                    break;
                case 4:
                    title = "Uncharted";
                    break;
                case 5:
                    title = "Uncharted 2";
                    break;
                case 6:
                    title = "Uncharted 3";
                    break;
                case 7:
                    title = "Uncharted 4";
                    break;
                case 8:
                    title = "How To Be a Gumpy";
                    break;
                case 9:
                    title = "How To Fly";
                    break;
                case 10:
                    title = "How To Jump High";
                    break;
                case 11:
                    title = "How To Cook";
                    break;
                case 12:
                    title = "Back To Basics";
                    break;
                case 13:
                    title = "Learn C#";
                    break;
                case 14:
                    title = "Learn Typescript";
                    break;
                case 15:
                    title = "Learn Javascript";
                    break;
                case 16:
                    title = "How To Fight a Dragon";
                    break;
                case 17:
                    title = "World of Warcraft: The Complete Edition";
                    break;
                case 18:
                    title = "How To Play Everquest";
                    break;
                case 19:
                    title = "Be a YouTuber";
                    break;
                case 20:
                    title = "Don't Be a Never Trumper";
                    break;
            }

            return title;

        }




        private string GetProductMedia()
        {
            Random random = new Random();
            int rndNum = random.Next(0, 21);
            string media = string.Empty;

            switch (rndNum)
            {
                case 0:
                    media = "//player.vimeo.com/video/173192945?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 1:
                    media = "//player.vimeo.com/video/179169563?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 2:
                    media = "//player.vimeo.com/video/179479722?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 3:
                    media = "//player.vimeo.com/video/179768736?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 4:
                    media = "//player.vimeo.com/video/180028667?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 5:
                    media = "//player.vimeo.com/video/195471382?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 6:
                    media = "//player.vimeo.com/video/195492285?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 7:
                    media = "//player.vimeo.com/video/195494203?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 8:
                    media = "//player.vimeo.com/video/195506334?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 9:
                    media = "//player.vimeo.com/video/219797629?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 10:
                    media = "//player.vimeo.com/video/219802319?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 11:
                    media = "//player.vimeo.com/video/219803440?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 12:
                    media = "//player.vimeo.com/video/219807809?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 13:
                    media = "//player.vimeo.com/video/242450172?title=0&byline=0&portrait=0&color=ffffff";
                    break;
                case 14:
                    media = "https://player.vimeo.com/video/218732620";
                    break;
                case 15:
                    media = "https://player.vimeo.com/video/262926486";
                    break;
                case 16:
                    media = "https://player.vimeo.com/video/264188894";
                    break;
                case 17:
                    media = "https://www.youtube.com/embed/1AI6RS1st2E";
                    break;
                case 18:
                    media = "https://www.youtube.com/embed/3ZEu6ZOMhlw";
                    break;
                case 19:
                    media = "https://www.youtube.com/embed/EgUGaHwk8PY";
                    break;
                case 20:
                    media = "https://www.youtube.com/embed/NIpvnY7fG-U";
                    break;
            }

            return media;

        }


        private void RateReview(Product product)
        {
            Random random = new Random();

            if (random.Next(2) == 1)
            {
                product.ProductReviews.ToList().ForEach((ProductReview review) =>
                {
                    int result = random.Next(2);
                    if (result == 0)
                    {
                        review.Dislikes++;
                    }
                    else
                    {
                        review.Likes++;
                    }
                    unitOfWork.ProductReviews.Update(review);
                });
            }
        }

        private async Task SetCollaboratorsAsync(string customerId, List<Product> products)
        {
            Random random = new Random();

            int numLists = random.Next(1, 4);

            List<List> lists = (List<List>)await unitOfWork.Lists.GetCollection();

            for (int i = 0; i < numLists; i++)
            {
                string listId = lists[random.Next(lists.Count)].Id;
                if (!await unitOfWork.Collaborators.Any(x => x.CustomerId == customerId && x.ListId == listId))
                {
                    Guid collaboratorId = Guid.NewGuid();

                    unitOfWork.Collaborators.Add(new ListCollaborator
                    {
                        Id = collaboratorId,
                        CustomerId = customerId,
                        ListId = listId,
                        IsOwner = false
                    });

                    await unitOfWork.Save();

                    await SetListProductsAsync(collaboratorId, products);

                    
                }
            }

        }


    }
}