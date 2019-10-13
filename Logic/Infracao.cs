using System.Linq;
using System.Linq.Expressions;
using System;
using Filesystem;
using Logic.Models;
using System.Collections.Generic;

namespace Logic
{
    public class Infracao
    {
        public static int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            return infracoes.Documents.Last().Id;
        }

        public static InfracaoModel FindInfracao(Predicate<InfracaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            return infracoes.Documents.Find(filter);
        }
        public static List<InfracaoModel> FindAllInfracao(Predicate<InfracaoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            return infracoes.Documents.FindAll(filter);
        }
        public static void InsertInfracao(InfracaoModel infracao)
        {
            int i = GetLastId();
            i++;
            infracao.Id = i;
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            infracoes.InsertDocument(infracao);
        }

        public static void DeleteInfracao(Expression<Func<InfracaoModel,bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            infracoes.DeleteDocument(filter);
        }
        public static void DeleteInfracao(InfracaoModel infracao)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            infracoes.DeleteDocument(x => x == infracao);
        }
        public static void UpdateInfracao(Expression<Func<InfracaoModel,bool>> filter, InfracaoModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local",s);
            Collection<InfracaoModel> infracoes = new Collection<InfracaoModel>("infra",local);
            infracoes.UpdateDocument(filter,update);
        }
    }
}
