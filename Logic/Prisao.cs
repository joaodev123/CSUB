using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Logic.Models;
using Filesystem;

namespace Logic
{
    public class Prisao : IDatabaseModel<PrisaoModel>
    {
        public void Delete(PrisaoModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            InfracaoModel infra = new Infracao().Find(x => x.PrisaoId == item.Id);
            infra.PrisaoId = 0;
            infra.Preso = false;
            infracoes.UpdateDocument(x => x.Id == infra.Id, infra);
            prisao.DeleteDocument(x => x.Id == item.Id);
        }

        public PrisaoModel Find(Predicate<PrisaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            return prisao.Documents.Find(filter);
        }

        public List<PrisaoModel> FindAll(Predicate<PrisaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            return prisao.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            var p = new List<PrisaoModel>();
            if (prisao.Documents != null) p = prisao.Documents;
            return p.Count;
        }

        public void Insert(PrisaoModel item)
        {
            int i = GetLastId();
            i++;
            item.Id = i;
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra", local);
            InfracaoModel infra = new Infracao().Find(x => x.PrisaoId == item.Id);
            infra.PrisaoId = item.Id;
            infra.Preso = true;
            infracoes.UpdateDocument(x => x.Id == infra.Id, infra);
            prisao.InsertDocument(item);
        }

        public void Update(Expression<Func<PrisaoModel, bool>> filter, PrisaoModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<PrisaoModel> prisao = new Collection<PrisaoModel>("prisao", local);
            prisao.UpdateDocument(filter, update);
        }
    }
}