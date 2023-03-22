using AutoMapper;
using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace DataRooms.UI
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here  
            //This is the important line to edit  
            //container.RegisterType<IMapper, Mapper>();

            var config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<File, FilewithPermission>();
                cfg.CreateMap<Folder, FolderwithPermission>();

                //...
            });

            IMapper mapper = config.CreateMapper();

            container.RegisterInstance(mapper);

            RegisterTypes(container);
            return container;
        }
        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}