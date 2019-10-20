using System.Linq;
using System.Linq.Expressions;
using System;
using Filesystem;
using Logic.Models;
using System.Collections.Generic;

namespace Logic
{
    public class Infracao : IDatabaseModel<InfracaoModel>
    {
        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            if (infracoes.Documents.Count > 0) return infracoes.Documents.Last().Id;
            else return 0;
        }

        public InfracaoModel Find(Predicate<InfracaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            return infracoes.Documents.Find(filter);
        }
        public List<InfracaoModel> FindAll(Predicate<InfracaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            return infracoes.Documents.FindAll(filter);
        }
        public void Insert(InfracaoModel infracao)
        {
            int i = GetLastId();
            i++;
            infracao.Id = i;
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<MembroModel> membro = new Collection<MembroModel>("membros", local);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            infracoes.InsertDocument(infracao);
            if (membro.Documents.Any(x => x.DiscordId == infracao.IdInfrator))
            {
                MembroModel update = membro.Documents.Find(x => x.DiscordId == infracao.IdInfrator);
                update.Infracoes.Add(i);
                new Membro().Update(x => x.DiscordId == infracao.IdInfrator, update);
            }
            else
            {
                int id = new Membro().GetLastId();
                id++;
                List<int> list = new List<int>();
                list.Add(i);
                MembroModel m = new MembroModel
                {
                    Id = id,
                    DiscordId = infracao.IdInfrator,
                    Infracoes = list
                };
                new Membro().Insert(m);
            }
        }

        public void Delete(Expression<Func<InfracaoModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            Collection<MembroModel> membro = new Collection<MembroModel>("membros", local);
            var infracao = (InfracaoModel)filter.Compile().Target;
            if (membro.Documents.Any(x => x.DiscordId == infracao.IdInfrator))
            {
                MembroModel update = membro.Documents.Find(x => x.DiscordId == infracao.IdInfrator);
                update.Infracoes.Remove(infracao.Id);
                new Membro().Update(x => x.DiscordId == infracao.IdInfrator, update);
            }
            infracoes.DeleteDocument(filter);
        }
        public void Delete(InfracaoModel infracao)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            Collection<MembroModel> membro = new Collection<MembroModel>("membros", local);
            if (membro.Documents.Any(x => x.DiscordId == infracao.IdInfrator))
            {
                MembroModel update = membro.Documents.Find(x => x.DiscordId == infracao.IdInfrator);
                update.Infracoes.Remove(infracao.Id);
                new Membro().Update(x => x.DiscordId == infracao.IdInfrator, update);
            }
            infracoes.DeleteDocument(x => x.Id == infracao.Id);
        }
        public void Update(Expression<Func<InfracaoModel, bool>> filter, InfracaoModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            infracoes.UpdateDocument(filter, update);
        }
    }
}
