﻿using FlatRedBall.Glue.Events;
using FlatRedBall.Glue.Managers;
using FlatRedBall.Glue.Reflection;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.IO;
using Gum.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GumPlugin.Managers
{
    public class EventsManager : Singleton<EventsManager>
    {
        List<FlatRedBall.Glue.Events.EventSave> mEventsAdded = new List<FlatRedBall.Glue.Events.EventSave>();

        internal void HandleAddEventsForObject(NamedObjectSave namedObject, List<ExposableEvent> listToFill)
        {
            if (namedObject.SourceType == SourceType.File)
            {
                string extension = FileManager.GetExtension(namedObject.SourceFile);

                if (extension == GumProjectSave.ComponentExtension ||
                    extension == GumProjectSave.ProjectExtension ||
                    extension == GumProjectSave.ScreenExtension ||
                    extension == GumProjectSave.StandardExtension)
                {
                    string strippedName = FileManager.RemoveExtension(FileManager.RemovePath(namedObject.SourceFile));

                    var element = AppState.Self.AllLoadedElements.FirstOrDefault(item=>
                        item.Name.ToLowerInvariant() == strippedName.ToLowerInvariant());

                    if(element != null)
                    {
                        string instanceName = namedObject.SourceNameWithoutParenthesis;

                        // Click
                        // RollOver
                        // RollOn
                        // RollOff

                        bool shouldAddEvents = false;

                        if (instanceName == "this")
                        {
                            shouldAddEvents = element is ComponentSave;
                        }
                        else
                        {

                            var instance = element.Instances.FirstOrDefault(item => item.Name == instanceName);

                            var instanceElement = Gum.Managers.ObjectFinder.Self.GetElementSave(instance);

                            shouldAddEvents = instanceElement != null && instanceElement is ComponentSave;
                        }

                        if(shouldAddEvents)
                        {
                            listToFill.Add(new ExposableEvent("Click"));
                            listToFill.Add(new ExposableEvent("RollOver"));
                            listToFill.Add(new ExposableEvent("RollOn"));
                            listToFill.Add(new ExposableEvent("RollOff"));
                        }
                    }
                }

            }

        }

        internal void RefreshEvents()
        {
            var allGlueEvents = FlatRedBall.Glue.Events.EventManager.AllEvents;
            var kvpList = allGlueEvents.Where(item => mEventsAdded.Contains(item.Value)).ToList();

            foreach (var key in kvpList.Select(item => item.Key))
            {
                allGlueEvents.Remove(key);
            }

            // Now let's re-add
            foreach (var element in AppState.Self.AllLoadedElements)
            {
                foreach (var gumEvent in element.Events.Where(item => item.Enabled))
                {
                    if (!allGlueEvents.ContainsKey(gumEvent.GetExposedOrRootName()))
                    {
                        var glueEventSave = new FlatRedBall.Glue.Events.EventSave();

                        glueEventSave.Arguments = "FlatRedBall.Gui.IWindow callingWindow";
                        glueEventSave.DelegateType = "FlatRedBall.Gui.WindowEvent";


                        allGlueEvents.Add(gumEvent.GetExposedOrRootName(), glueEventSave);
                    }

                }
            }

        }
    }
}
