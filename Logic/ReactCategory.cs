using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Filesystem;
using Logic.Models;

namespace Logic
{
    public class ReactCategory : IDatabaseModel<ReactCategoryModel>
    {
        public void Delete(ReactCategoryModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            var data = item;
            if (data.Roles != null)
            {
                foreach (int i in data.Roles)
                {
                    new ReactRole().Delete(new ReactRole().Find(x => x.Id == i));
                }
            }
            var channel = new ReactChannel().Find(x => x.DiscordID == data.ChannelId);
            channel.Categories.Remove(data.Id);
            new ReactChannel().Update(x => x.Id == channel.Id, channel);
            category.DeleteDocument(x => x.Id == data.Id);
        }

        public ReactCategoryModel Find(Predicate<ReactCategoryModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            return category.Documents.Find(filter);
        }

        public List<ReactCategoryModel> FindAll(Predicate<ReactCategoryModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            return category.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            if (category.Documents.Count > 0) return category.Documents.Last().Id;
            else return 0;
        }

        public void Insert(ReactCategoryModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            int id = GetLastId();
            id++;
            if (item.ChannelId != 0)
            {
                var channel = new ReactChannel().Find(x => x.DiscordID == item.ChannelId);
                if (channel.Categories == null) { channel.Categories = new List<int>(); }
                channel.Categories.Add(id);
                new ReactChannel().Update(x => x.Id == channel.Id, channel);
                item.Id = id;
                category.InsertDocument(item);
            }
        }

        public void Update(Expression<Func<ReactCategoryModel, bool>> filter, ReactCategoryModel update)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactCategoryModel> category = new Collection<ReactCategoryModel>("category", react);
            category.UpdateDocument(filter, update);
        }
    }
}