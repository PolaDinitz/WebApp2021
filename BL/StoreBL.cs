using System.Collections.Generic;
using System.Linq;
using WebApp2021.Models;
using WebApp2021.DAL;
using System.Threading.Tasks;
using System;

namespace WebApp2021.BL
{
    public class StoreBL
    {
        private readonly AppDbContext db;
       
        public StoreBL(AppDbContext db) => this.db = db;


        public IEnumerable<Store> GetStores(Func<Store, bool> predicate)
        {
            if (predicate != null)
            {
                return this.db.Stores.Where(predicate).AsEnumerable();
            }

            return this.db.Stores.AsEnumerable();
        }
        public ValueTask<Store> GetStoreById(int id)
        {
            return this.db.Stores.FindAsync(id);
        }

        public Task<int> AddStore(Store store)
        {
            this.db.Stores.Add(store);
            return this.db.SaveChangesAsync();
        }

        public Task<int> UpdateStore(Store store)
        {
            this.db.Stores.Update(store);
            return this.db.SaveChangesAsync();
        }

        public async Task<int> DeleteStore(int id)
        {
            Store store = await this.GetStoreById(id);

            this.db.StoreTags.RemoveRange(
               this.db.StoreTags
               .Where(ri => ri.StoreId == id).ToList());

            store.Tags.Clear();

            db.Stores.Remove(store);

            return await db.SaveChangesAsync();       
        }

        public ICollection<Tag> GetTags()
        {
            return this.db.Tags.ToList();
        }

        public ICollection<Tag> GetStoreTags(int storeID)
        {
            return this.db.Stores.Find(storeID)
                .Tags.Select(ri => ri.Tag).ToList();
        }
    }
}