﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Instructions.Reflection;
using FlatRedBall.Glue.Parsing;
using FlatRedBall.IO;

namespace FlatRedBall.Glue.Elements
{
    public static class AssetTypeInfoExtensionMethods
    {       

        public static List<TypedMemberBase> GetTypedMembers(this AssetTypeInfo thisAti)
        {
            List<TypedMemberBase> typedMembers = new List<TypedMemberBase>();

            foreach (var item in thisAti.CachedExtraVariables)
            {
                TypedMemberBase typedMemberBase = GetTypedMemberBase(item.Type, item.Member);

                typedMembers.Add(typedMemberBase);
                        
            }

            return typedMembers;
        }


        public static TypedMemberBase GetTypedMemberBase(string typeString, string memberName)
		{
			// make the typeString proper
            Type type = TypeManager.GetTypeFromString(typeString);

            // At one time this was using GetTypedMember, but I don't know why we need them
            // to be equatable
			//TypedMemberBase typedMemberBase = TypedMemberBase.GetTypedMember(memberName, type);
            TypedMemberBase typedMemberBase = TypedMemberBase.GetTypedMemberUnequatable(memberName, type);
			return typedMemberBase;
		}

        public static string QualifyBaseType(string typeString)
        {
            Type type = TypeManager.GetTypeFromString(typeString);

            if (type != null)
            {
                return type.FullName;
            }
            else
            {
                return typeString;
            }
        }
    }
}
