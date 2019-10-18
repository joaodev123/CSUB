using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Filesystem;
using Logic.Models;

namespace Logic
{
    public class Censo : IDatabaseModel<CensoModel>
    {
        public void Delete(Expression<Func<CensoModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            var c = (CensoModel)filter.Compile().Target;
            var membro = new Membro().Find(x => x.DiscordId == c.DiscordId);
            membro.Censo = 0;
            new Membro().Update(x => x.Id == membro.Id, membro);
            censo.DeleteDocument(filter);
        }

        public void Delete(CensoModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            var c = item;
            var membro = new Membro().Find(x => x.DiscordId == c.DiscordId);
            membro.Censo = 0;
            new Membro().Update(x => x.Id == membro.Id, membro);
            censo.DeleteDocument(x => x.Id == item.Id);
        }

        public CensoModel Find(Predicate<CensoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            return censo.Documents.Find(filter);
        }

        public List<CensoModel> FindAll(Predicate<CensoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            return censo.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            if (censo.Documents.Count > 0) return censo.Documents.Last().Id;
            else return 0;
        }

        public void Insert(CensoModel item)
        {
            int i = GetLastId();
            i++;
            item.Id = i;
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            Collection<MembroModel> membro = new Collection<MembroModel>("membros", local);
            if (!censo.Documents.Any(x => x.DiscordId == item.DiscordId))
            {
                censo.InsertDocument(item);
            }
            if (membro.Documents.Any(x => x.DiscordId == item.DiscordId))
            {
                var m = new Membro().Find(x => x.DiscordId == item.DiscordId);
                m.Censo = i;
                new Membro().Update(x => x.Id == m.Id, m);
            }
            else
            {
                var in2 = new Membro().GetLastId();
                in2++;
                MembroModel m = new MembroModel
                {
                    Id = in2,
                    DiscordId = item.DiscordId,
                    Censo = i
                };
                new Membro().Insert(m);
            }
        }

        public void Update(Expression<Func<CensoModel, bool>> filter, CensoModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<CensoModel> censo = new Collection<CensoModel>("censo", local);
            censo.UpdateDocument(filter, update);
        }
    }
}