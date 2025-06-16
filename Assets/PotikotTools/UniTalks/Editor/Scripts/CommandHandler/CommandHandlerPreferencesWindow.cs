using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PotikotTools.UniTalks.Editor
{
    public class CommandHandlerPreferencesWindow : EditorWindow
    {
        private Toggle _excludeAssemblyPrefixesToggle;
        private ListView _excludedFromSearchAssemblyPrefixesListView;
        private ListView _assembliesListView;
        
        [MenuItem("Tools/PotikotTools/Command Handler Preferences", priority = 100)]
        public static void Open()
        {
            GetWindow<CommandHandlerPreferencesWindow>("Command Handler Preferences");
        }

        public void CreateGUI()
        {
            _assembliesListView = CreateListView("Command Attribute Using Assemblies", CommandHandlerPreferences.CommandAttributeUsingAssemblies);
            _excludedFromSearchAssemblyPrefixesListView = CreateListView("Excluded From Search Assembly Prefixes", CommandHandlerPreferences.ExcludedFromSearchAssemblyPrefixes);
            
            VisualElement container = new()
            {
                style =
                {
                    marginTop = 5f,
                    marginLeft = 10f,
                    marginRight = 5f
                }
            };

            VisualElement excludedAssembliesContainer = new()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            
            _excludeAssemblyPrefixesToggle = new Toggle();
            _excludeAssemblyPrefixesToggle.RegisterValueChangedCallback(evt =>
            {
                _excludedFromSearchAssemblyPrefixesListView.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;

                CommandHandlerPreferences.ExcludeFromSearchAssemblies = evt.newValue;
                CommandHandlerPreferences.Save();
            });
            _excludeAssemblyPrefixesToggle.value = CommandHandlerPreferences.ExcludeFromSearchAssemblies;

            Label excludeAssemblyPrefixesLabel = new("Exclude Assembly Prefixes");
            
            excludedAssembliesContainer.Add(_excludeAssemblyPrefixesToggle);
            excludedAssembliesContainer.Add(excludeAssemblyPrefixesLabel);

            Button searchAssembliesButton = new(SearchDeveloperAssemblies)
            {
                text = "Search Developer Assemblies"
            };

            container.Add(excludedAssembliesContainer);
            container.Add(_excludedFromSearchAssemblyPrefixesListView);
            
            container.Add(CreateSpace(10f));
            container.Add(_assembliesListView);
            container.Add(CreateSpace(10f));
            container.Add(searchAssembliesButton);
            container.Add(new Button(ResetPreferences) { text = "Reset" });

            rootVisualElement.Add(container);
        }

        private void SearchDeveloperAssemblies()
        {
            CommandHandlerPreferences.CommandAttributeUsingAssemblies.Clear();

            foreach (var assembly in CommandHandlerUtility.GetCommandUserAssemblies())
                CommandHandlerPreferences.CommandAttributeUsingAssemblies.Add(assembly.FullName);

            CommandHandlerPreferences.Save();
            _assembliesListView.Rebuild();
        }

        private void ResetPreferences()
        {
            CommandHandlerPreferences.Reset();
            
            _excludeAssemblyPrefixesToggle.SetValueWithoutNotify(CommandHandlerPreferences.ExcludeFromSearchAssemblies);

            _assembliesListView.itemsSource = CommandHandlerPreferences.CommandAttributeUsingAssemblies;
            _assembliesListView.bindItem = (e, i) => BindListItem(CommandHandlerPreferences.CommandAttributeUsingAssemblies, e, i);
            _assembliesListView.Rebuild();
            
            _excludedFromSearchAssemblyPrefixesListView.itemsSource = CommandHandlerPreferences.ExcludedFromSearchAssemblyPrefixes;
            _excludedFromSearchAssemblyPrefixesListView.bindItem = (e, i) => BindListItem(CommandHandlerPreferences.ExcludedFromSearchAssemblyPrefixes, e, i);
            _excludedFromSearchAssemblyPrefixesListView.Rebuild();
        }

        private static ListView CreateListView(string headerTitle, List<string> source)
        {
            ListView listView = new(source)
            {
                headerTitle = headerTitle,
                showFoldoutHeader = true,
                showBorder = true,
                showAddRemoveFooter = true,
                // reorderable = true,
                // reorderMode = ListViewReorderMode.Animated,
                makeItem = MakeListItem,
                bindItem = (e, i) => BindListItem(source, e, i)
            };

            listView.itemsAdded += _ => CommandHandlerPreferences.Save();
            listView.itemsRemoved += _ => CommandHandlerPreferences.Save();

            return listView;
        }

        private static VisualElement MakeListItem()
        {
            TextField textField = new();
            return textField;
        }

        private static void BindListItem(List<string> source, VisualElement element, int index)
        {
            TextField textField = element.Q<TextField>();
            textField.label = $"Element {index}";
            textField.value = source[index]; // TODO: index out of range on reset empty list
            
            textField.RegisterValueChangedCallback(evt =>
            {
                source[index] = evt.newValue;
                CommandHandlerPreferences.Save();
            });
        }
        
        private static VisualElement CreateSpace(float height)
        {
            return new VisualElement()
            {
                style =
                {
                    height = height
                }
            };
        }
    }
}