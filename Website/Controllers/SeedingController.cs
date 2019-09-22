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
        public async Task<ActionResult> Post(List<Account> accounts)
        {
            foreach (Account account in accounts)
            {
                Customer customer = await CreateAccount(account);



                List<Product> products = (List<Product>)await unitOfWork.Products.GetCollection();

                SetProductOrders(customer.Id, products);


                await unitOfWork.Save();



                //foreach (Product product in products)
                //{
                //    WriteReview(product, customer.Id);

                //    await unitOfWork.Save();

                //}

            }













            return Ok();


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

            if(rating > 0)
            {
                switch (rating)
                {
                    case 1:
                        title = "Clunky and Boring";
                        text = "Took a chance given the 5 star rating and regret spending the $20 bucks. Boring is the first word that comes to mind, as I wade through the storyline and struggle to figure out which button and/or stick controls which action. I guess there are those who this type of post-apocalyptic struggle to survive warn out genre appeals to, but if you are going to live in that world, then get on with the shooting and maiming and limit the half-cocked storyline.";
                        break;
                    case 2:
                        title = "Disappointed in the game play";
                        text = "Great storyline that was engaging to start. But once the game play actually got going, it was incredibly linear. You're trying to escape from guys at night but are forced to go through the trench and then the only building that they happen to be occupying?? In a world with so many sandbox like game engines, I just could not get into such a linear game. It was too bad as the story sounded very interesting, but not enough for me to sink hours of annoying game play into.";
                        break;
                    case 3:
                        title = "Visually Amazing. Atmosphere Fantastic. Gameplay Middling.";
                        text = "I sort of enjoyed the game except for one nagging issue throughout its entirety: it was all sneaking around a bunch of zombies. There was just way too much sneaking past everything, with no other options, for my liking. I had the same feeling with the Alien game where everywhere I went that stupid alien would show up immediately and then wander around causing me to have to sneak all over the place. Annoying... that would be my primary summary of the game. Run and gun would have been boring too, though. There simply isn't enough variety in dealing with the encounters for me. No amount of visuals or storytelling can account for that lack in the gameplay.";
                        break;
                    case 4:
                        title = "It should have always been a Next-Gen Game!";
                        text = "The game plays out in standard third-person adventure fashion, and it’s very linear. But compelling controls and near film-quality visuals drive a masterful experience. Both those things have gotten better in the remastered edition. The game has been completely redone in 1080p, and the difference between PS3 and PS4 versions is huge.You’ll read signs in the distance now, and see the skyline and far off buildings with far greater clarity. A handful of iconic scenes now truly jump off the screen, immersing you more fully in the action. Its worth a shot!";
                        break;
                    case 5:
                        title = "Converted me to a PS4 fanboy";
                        text = "I never had a playstation 2 or 3 so I missed out on this game when it first came out. My roommate bought a PS4 so I thought I'd try this out considering all the praise it received. It looks fantastic, if I didn't know it was remastered I wouldn't have thought it was from a previous generation console. Story is well written and the animation and modeling on the characters is very realistic. The gameplay gets a little repetitive by the end but I really enjoyed having to make it through certain levels with only a brick and a shiv. The crafting system is just the right balance of making exploring the environments rewarding while also forcing you to conserve every bullet. I died a lot. If I had played this back in 2013 I would've thought it was one of the best games ever made. Playing it now isn't nearly as jaw-dropping but I can still appreciate how well-polished everything is. Now I'm really looking forward to the Last of Us II.";
                        break;
                }





                ProductReview review = new ProductReview
                {
                    Title = title,
                    Rating = rating,
                    Text = text
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


            for(int j = 0; j < numOrders; j++)
            {
                string orderId = Guid.NewGuid().ToString("N").Substring(0, 21).ToUpper();
                double subtotal = Math.Round(random.NextDouble() * 50, 2);
                double shipping = Math.Round(random.NextDouble() * 5, 2);
                double discount = Math.Round(random.NextDouble() * 2, 2);
                double tax = Math.Round(random.NextDouble() * 3, 2);
                double total = subtotal + shipping - discount + tax;
                Product product = products[random.Next(0, products.Count)];

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
                    OrderProducts = GetOrderProducts(orderId, product)
                };

                unitOfWork.ProductOrders.Add(productOrder);
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

            for(int i = 0; i < numProducts; i++)
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 25).ToUpper(),
                    OrderId = orderId,
                    Title = i == 0 ? product.Title : GetProductTitle(),
                    Type = random.Next(0, 3),
                    Quantity = random.Next(1, 3),
                    Price = Math.Round(random.NextDouble() * 9.99, 2),
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

    }
}