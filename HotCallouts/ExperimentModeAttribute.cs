﻿using LCPD_First_Response.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotCallouts
{
    public enum ElementType
    {
        WorldEvent,
        Plugin,
        Callout
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExperimentModeAttribute : System.Attribute
    {
        private static List<Type> ExperimentCallouts_ = new List<Type>();
        private static List<Type> ExperimentWorldEvents_ = new List<Type>();

        public static List<Type> ExperimentCallouts { 
            get
            {
                return ExperimentCallouts_;
            }
            set
            {
                ExperimentCallouts_ = value;
            }
        }
        public static List<Type> ExperimentWorldEvents
        {
            get
            {
                return ExperimentWorldEvents_;
            }
            set
            {
                ExperimentWorldEvents_ = value;
            }
        }

        public ExperimentModeAttribute(Type yourself, ElementType type)
        {
            if(yourself == null || type == null)
            {
                Log.Error("Exception thrown: ExperimentModeAttribute argument null", "HighHot");
                throw new ArgumentNullException();
            }
            switch(type)
            {
                case ElementType.Callout :
                    ExperimentCallouts.Add(yourself);           
                    break;
                case ElementType.WorldEvent :
                    ExperimentWorldEvents.Add(yourself);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
