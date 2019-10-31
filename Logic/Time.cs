using System;
using Filesystem;
using System.Collections.Generic;
using System.Linq.Expressions;
using Logic.Models;

namespace Logic
{
    public class Time : IDatabaseModel<TimeModel>
    {
        public void Delete(Expression<Func<TimeModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            TimeModel dados = (TimeModel)filter.Compile().Target;
            EventoModel evento = eventos.Documents.Find(x => x.Id == dados.EventoId);
            evento.Times?.Remove(dados.Id);
            eventos.UpdateDocument(x => x.Id == dados.EventoId, evento);
            times.DeleteDocument(x => x.Id == dados.Id);
        }

        public void Delete(TimeModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            TimeModel dados = item;
            EventoModel evento = eventos.Documents.Find(x => x.Id == dados.EventoId);
            evento.Times?.Remove(dados.Id);
            eventos.UpdateDocument(x => x.Id == dados.EventoId, evento);
            times.DeleteDocument(x => x.Id == dados.Id);
        }

        public TimeModel Find(Predicate<TimeModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            return times.Documents.Find(filter);
        }

        public List<TimeModel> FindAll(Predicate<TimeModel> filter)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            return times.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            if (times.Documents != null) return times.Documents.Count;
            else return 0;
        }

        public void Insert(TimeModel item)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            int i = GetLastId();
            i++;
            item.Id = i;
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            Collection<EventoModel> eventos = new Collection<EventoModel>("eventos", local);
            EventoModel evento = eventos.Documents.Find(x => x.Id == item.EventoId);
            if (evento.Times == null) evento.Times = new List<int>();
            evento.Times.Add(item.Id);
            new Evento().Update(x => x.Id == item.EventoId, evento);
            times.InsertDocument(item);
        }

        public void Update(Expression<Func<TimeModel, bool>> filter, TimeModel update)
        {
            Server s = Server.Instance();
            Database local = new Database("local", s);
            Collection<TimeModel> times = new Collection<TimeModel>("times", local);
            times.UpdateDocument(filter, update);
        }
    }
}