using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Logic.Models;
using Filesystem;
using System.Linq;

namespace Logic
{
    public class Membro : IDatabaseModel<MembroModel>
    {
        public void Delete(Expression<Func<MembroModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            var membro = (MembroModel)filter.Compile().Target;
            var infras = new Infracao().FindAll(x => x.IdInfrator == membro.DiscordId);
            infras.ForEach(x => new Infracao().Delete(x));
            membros.DeleteDocument(filter);
        }

        public void Delete(MembroModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            var infras = new Infracao().FindAll(x => x.IdInfrator == item.DiscordId);
            infras.ForEach(x => new Infracao().Delete(x));
            membros.DeleteDocument(x => x.Id == item.Id);
        }

        public MembroModel Find(Predicate<MembroModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            return membros.Documents.Find(filter);
        }

        public List<MembroModel> FindAll(Predicate<MembroModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            return membros.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            if(membros.Documents.Count > 0) return membros.Documents.Last().Id;
            else return 0;
        }

        public void Insert(MembroModel item)
        {
            int i = GetLastId();
            i++;
            item.Id = i;
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            if (!membros.Documents.Any(x => x.DiscordId == item.DiscordId))
            {
                membros.InsertDocument(item);
            }
        }

        public void Update(Expression<Func<MembroModel, bool>> filter, MembroModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<MembroModel> membros = new Collection<MembroModel>("membros", local);
            membros.UpdateDocument(filter,update);
        }
    }
}