﻿using FlatRedBall.IO;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using StateAnimationPlugin.SaveClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GumPlugin.CodeGeneration
{
    public static class SaveObjectExtensionMethods
    {
        public static string MemberNameInCode(this InstanceSave instanceSave)
        {
            return InstanceNameInCode(instanceSave.Name);
        }

        public static string InstanceNameInCode(string name)
        {
            return FlatRedBall.IO.FileManager.RemovePath(name.Replace(" ", "_").Replace("-", "_"));
        }

        public static string MemberNameInCode(this StateSave stateSave)
        {
            return stateSave.Name.Replace(" ", "_").Replace("-", "_");
        }

        public static string MemberTypeInCode(this ElementSave element)
        {
            return FlatRedBall.IO.FileManager.RemovePath(element.Name);
        }

        public static string PropertyNameInCode(this AnimationSave animation)
        {
            string propertyName = animation.Name + "Animation";


            var firstChar = propertyName.Substring(0, 1).ToUpperInvariant();

            return firstChar + propertyName.Substring(1);
        }

        public static string PropertyNameInCode(this AnimationReferenceSave animation)
        {
            string upperCaseRootName;

            var firstChar = animation.RootName.Substring(0, 1).ToUpperInvariant();

            upperCaseRootName = firstChar + animation.RootName.Substring(1) + "Animation";

            if (string.IsNullOrEmpty(animation.SourceObject))
            {
                return upperCaseRootName;
            }
            else
            {
                return FileManager.RemovePath(animation.SourceObject) + "." + upperCaseRootName;
            }
        }
        


        public static string MemberNameInCode(this VariableSave variableSave, ElementSave container, Dictionary<string, string> replacements)
        {
            var rootName = variableSave.GetRootName();
            var objectName = variableSave.SourceObject;

            if (replacements.ContainsKey(rootName))
            {
                rootName = replacements[rootName];
            }
            else
            {
                rootName = rootName.Replace(" ", "_");
            }

            ElementSave throwaway1;
            StateSaveCategory throwaway2;
            // recursive is false because we only want to prepend "Current" if it's not an exposed variable
            if (variableSave.IsState(container, out throwaway1, out throwaway2, recursive:false))
            {
                if (rootName == "State")
                {
                    rootName = "CurrentVariableState";

                }
                else
                {
                    rootName = "Current" + rootName;
                }
            }

            if (string.IsNullOrEmpty(objectName))
            {
                return rootName;
            }
            else
            {
                objectName = InstanceNameInCode( objectName);
                return objectName + '.' + rootName;
            }
        }

    }
}