using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Filesystem;
using Logic.Models;

namespace Logic
{
    public class Evento : IDatabaseModel<EventoModel>
    {
        public void Delete(Expression<Func<EventoModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            EventoModel model = (EventoModel)filter.Compile().Target;
            eventos.DeleteDocument(x => x.Id == model.Id);
        }

        public void Delete(EventoModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            eventos.DeleteDocument(x => x.Id == item.Id);
        }

        public EventoModel Find(Predicate<EventoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            return eventos.Documents.Find(filter);
        }

        public List<EventoModel> FindAll(Predicate<EventoModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            return eventos.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            if (eventos.Documents.Count > 0) return eventos.Documents.Count;
            else return 0;
        }

        public void Insert(EventoModel item)
        {
            int id = GetLastId();
            id++;
            item.Id = id;
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            eventos.InsertDocument(item);
        }

        public void Update(Expression<Func<EventoModel, bool>> filter, EventoModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            eventos.UpdateDocument(filter, update);
        }
    }
}