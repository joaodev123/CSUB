using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Filesystem;
using Logic.Models;

namespace Logic
{
    public class ReactRole : IDatabaseModel<ReactRoleModel>
    {
        public void Delete(Expression<Func<ReactRoleModel, bool>> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            var data = (ReactRoleModel)filter.Compile().Target;
            if (data.CategoryId != 0)
            {
                var category = new ReactCategory().Find(x => x.Id == data.CategoryId);
                category.Roles.Remove(data.Id);
                new ReactCategory().Update(x => x.Id == category.Id, category);
                role.DeleteDocument(x => x.Id == data.Id);
            }
        }

        public void Delete(ReactRoleModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            var data = item;
            if (data.CategoryId != 0)
            {
                var category = new ReactCategory().Find(x => x.Id == data.CategoryId);
                category.Roles.Remove(data.Id);
                new ReactCategory().Update(x => x.Id == category.Id, category);
                role.DeleteDocument(x => x.Id == data.Id);
            }
        }

        public ReactRoleModel Find(Predicate<ReactRoleModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            return role.Documents.Find(filter);
        }

        public List<ReactRoleModel> FindAll(Predicate<ReactRoleModel> filter)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            return role.Documents.FindAll(filter);
        }

        public int GetLastId()
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            if (role.Documents.Count > 0) return role.Documents.Last().Id;
            else return 0;
        }

        public void Insert(ReactRoleModel item)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            int i = GetLastId();
            i++;
            item.Id = i;
            if (item.CategoryId != 0)
            {
                var category = new ReactCategory().Find(x => x.Id == item.CategoryId);
                category.Roles.Add(item.Id);
                new ReactCategory().Update(x => x.Id == category.Id, category);
                role.InsertDocument(item);
            }
        }

        public void Update(Expression<Func<ReactRoleModel, bool>> filter, ReactRoleModel update)
        {
            Server s = Server.Instance();
            Database react = new Database("react", s);
            Collection<ReactRoleModel> role = new Collection<ReactRoleModel>("role", react);
            role.UpdateDocument(filter,update);
        }
    }
}