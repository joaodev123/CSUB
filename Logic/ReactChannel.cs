using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Filesystem;
using Logic.Models;

namespace Logic
{
    public class ReactChannel : IDatabaseModel<ReactChannelModel>
    {

        public void Delete(ReactChannelModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            var data = item;
            if (data.Categories != null)
            {
                foreach (int i in data.Categories)
                {
                    new ReactCategory().Delete(new ReactCategory().Find(x => x.Id == i));
                }
            }
            channel.DeleteDocument(x => x.Id == data.Id);
        }

        public ReactChannelModel Find(Predicate<ReactChannelModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            return channel.Documents.Find(filter);
        }

        public List<ReactChannelModel> FindAll(Predicate<ReactChannelModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            return channel.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            if (channel.Documents.Count > 0) return channel.Documents.Last().Id;
            else return 0;
        }

        public void Insert(ReactChannelModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            int id = GetLastId();
            id++;
            item.Id = id;
            if (!channel.Documents.Any(x => x.DiscordID == item.DiscordID))
            {
                channel.InsertDocument(item);
            }
        }

        public void Update(Expression<Func<ReactChannelModel, bool>> filter, ReactChannelModel update)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactChannelModel> channel = new Collection<ReactChannelModel>("channel", react);
            channel.UpdateDocument(filter, update);
        }
    }
}