using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;

namespace Website.Repositories
{
    public class ListRepository : Repository<List>, IListRepository
    {
        // Set the context
        private readonly NicheShackContext context;

        public ListRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }




        // ................................................................................Get Lists.....................................................................
        public async Task<IEnumerable<ListDTO>> GetLists(string customerId)
        {
            // Returns all of the customer's lists.
            return await context.ListCollaborators
                .AsNoTracking()
                .OrderByDescending(x => x.IsOwner)
                .Where(x => x.CustomerId == customerId)
                .Select(x => new ListDTO
                {
                    Id = x.ListId,
                    Name = x.List.Name,
                    Description = x.List.Description,
                    TotalItems = context.ListProducts
                        .Count(z => x.List.Collaborators
                            .Select(y => y.Id)
                            .Contains(z.CollaboratorId)),
                    Owner = x.List.Collaborators
                        .Where(y => y.ListId == x.ListId && y.IsOwner)
                        .Select(y => y.CustomerId == customerId ? "You" : y.Customer.FirstName)
                        .FirstOrDefault(),
                    CollaborateId = x.List.CollaborateId
                })
                .ToListAsync();
        }






        // ................................................................................Get List Products.....................................................................
        public async Task<IEnumerable<ListProductDTO>> GetListProducts(IEnumerable<Collaborator> collaborators, string customerId, string sort)
        {
            // Gets products based on collaborators from a list.
            var products = await context.ListProducts
                .AsNoTracking()
                .SortBy(new ListProductDTO(sort))
                .Where(x => collaborators
                    .Select(y => y.Id)
                    .Contains(x.CollaboratorId))
                .Select(x => new ListProductDTO
                {
                    Title = x.Product.Title,
                    Rating = x.Product.Rating,
                    TotalReviews = x.Product.TotalReviews,
                    MinPrice = x.Product.MinPrice,
                    MaxPrice = x.Product.MaxPrice,
                    DateAdded = x.DateAdded.ToString("MMMM dd, yyyy"),
                    Collaborator = x.Collaborator.CustomerId == customerId ? "you" : x.Collaborator.Customer.FirstName,
                    Hoplink = x.Product.Hoplink,
                    Image = x.Product.Image,
                    UrlTitle = x.Product.UrlTitle
                })
                .ToListAsync();

            return products;
        }
    }
}
