// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;
using FluentAssertions;
using MsBw.MsBwUtility.JetBrains.Annotations;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.ListViewItems;
using TestStack.White.UIItems.WPFUIItems;
using TestStack.White.WindowsAPI;
using Button = TestStack.White.UIItems.Button;
using ListView = TestStack.White.UIItems.ListView;
using TextBox = TestStack.White.UIItems.TextBox;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    [UsedImplicitly]
    public class GridSteps : WpfBaseSteps
    {
        [Then(@"there is a grid under the label '(.*)'")]
        public void ThenThereIsAGridUnderTheLabel(string labelText)
        {
            FindAndSetGrid(labelText);
        }

        private void FindAndSetGrid(string labelText)
        {
            var grid = LocateClosestElementOfType<ListView>(labelText);
            grid
                .Should()
                .NotBeNull();
            Context.Grid = grid;
        }

        [When(@"I use the grid under the label '(.*)'")]
        public void WhenIUseTheGridUnderTheLabel(string labelText)
        {
            FindAndSetGrid(labelText);
        }

        [Then(@"that grid has (\d+) rows")]
        public void ThenThatGridHasRows(int numberOfRows)
        {
            Context.Grid.Rows
                        .Should()
                        .HaveCount(numberOfRows);
        }

        [When(@"I use the combobox in column '(.*)' on row (.*)")]
        public void WhenIUseTheComboboxInColumnOnRow(string columnName,
                                                     int rowIndex)
        {
            var headerColumn = GetHeaderColumn(columnName);
            ActionWhenIUseTheComboboxInColumnOnRow(headerColumn.Index,
                                                   rowIndex);
        }

        private void ActionWhenIUseTheComboboxInColumnOnRow(int columnIndex,
                                                            int rowIndex)
        {
            WhenFormat(@"I use the combobox in column {0} on row {1}",
                       columnIndex,
                       rowIndex);
        }

        [When(@"I use the combobox in column (\d+) on row (.*)")]
        public void WhenIUseTheComboboxInColumnOnRow(int columnIndex,
                                                     int rowIndex)
        {
            FindAndSetComboBox(columnIndex,
                               rowIndex);
        }

        [Then(@"there is a combobox in column '(.*)' on row (.*)")]
        public void ThenThereIsAComboboxInColumnOnRow(string columnName,
                                                      int rowIndex)
        {
            var headerColumn = GetHeaderColumn(columnName);
            ActionThenThereIsAComboboxInColumnOnRow(headerColumn.Index,
                                                    rowIndex);
        }

        private void ActionThenThereIsAComboboxInColumnOnRow(int columnIndex,
                                                             int gridRow)
        {
            ThenFormat(@"there is a combobox in column {0} on row {1}",
                       columnIndex,
                       gridRow);
        }

        [Then(@"there is a combobox in column (\d+) on row (\d+).*")]
        public void ThenThereIsAComboboxInColumnOnRow(int columnIndex,
                                                      int gridRow)
        {
            FindAndSetComboBox(columnIndex,
                               gridRow);
        }

        private void FindAndSetComboBox(int columnIndex,
                                               int gridRow)
        {
            var box = FindWidgetIn<WPFComboBox>(columnIndex,
                                                gridRow);
            Context.ComboBox = box;
        }

        private TWidgetType FindWidgetIn<TWidgetType>(int columnIndex,
                                                             int gridRow) where TWidgetType : UIItem
        {
            var row = GetRow(gridRow);
            var headerCellBounds = Context.Grid.Header.Columns[columnIndex].Bounds;
            var widgetType = typeof (TWidgetType);
            var widgetsOnRow = row.GetMultiple(SearchCriteria.ByControlType(testControlType: widgetType,
                                                                            framework: WindowsFramework.Wpf))
                                  .Cast<TWidgetType>();
            var widget =
                widgetsOnRow.FirstOrDefault(b =>
                                            b.Bounds.TopLeft.X >= headerCellBounds.TopLeft.X &&
                                            b.Bounds.TopRight.X <= headerCellBounds.TopRight.X);
            widget.Should()
                  .NotBeNull("Expected to find a {0} within the given column",
                             widgetType.Name);
            return widget;
        }

        private ListViewRow GetRow(int gridRow)
        {
            var listViewRows = ValidateRowIndex(gridRow);
            var row = listViewRows[gridRow];
            return row;
        }

        private ListViewRows ValidateRowIndex(int gridRow)
        {
            var listViewRows = Context.Grid.Rows;
            gridRow
                .Should()
                .BeLessThan(listViewRows.Count,
                            "Can't access a row at zero based index >= the number of rows in the grid");
            return listViewRows;
        }

        [When(@"I press the '(.*)' key in row (.*) of that grid")]
        public void WhenIPressTheKeyInRowOfThatGrid(KeyboardInput.SpecialKeys key,
                                                    int rowNumber)
        {
            var row = GetRow(rowNumber);
            row.Select();
            row.KeyIn(key);
        }

        [StepArgumentTransformation(@"\[(.*)\]")]
        public KeyboardInput.SpecialKeys GetKey(string key)
        {
            return (KeyboardInput.SpecialKeys) Enum.Parse(enumType: typeof (KeyboardInput.SpecialKeys),
                                                          value: key,
                                                          ignoreCase: true);
        }

        [When(@"I select row (.*) of the grid")]
        public void WhenISelectRowOfTheGrid(int gridRow)
        {
            var row = GetRow(gridRow);
            row.Select();
        }

        public static string ActionThenThereIsAButtonOnRow(string buttonText,
                                                           int rowIndex)
        {
            return string.Format(@"there is a '{0}' button on row {1}",
                                 buttonText,
                                 rowIndex);
        }

        [Then(@"there is a '(.*)' button on row (.*)")]
        public void ThenThereIsAButtonInColumnOnRow(string buttonText,
                                                    int rowIndex)
        {
            var row = GetRow(rowIndex);
            var button = row.Get<Button>(SearchCriteria.ByClassName(typeof (Button).Name)
                                                       .AndByText(buttonText));
            Context.Button = button;
        }

        [When(@"I type '(.*)' into grid column '(.*)'")]
        public void WhenITypeIntoGridColumn(string text,
                                            string columnName)
        {
            var headerColumn = GetHeaderColumn(columnName);
            ActionWhenITypeIntoTheGridCell(text,
                                           headerColumn.Index);
        }

        private ListViewColumn GetHeaderColumn(string columnName)
        {
            var header = Context.Grid.Header;
            var headerColumn = header.Column(columnName);
            if (headerColumn != null) return headerColumn;
            var validColumns = header.Columns.Select(c => c.Name)
                                     .Where(nm => !string.IsNullOrEmpty(nm))
                                     .Aggregate((c1,
                                                 c2) => c1 + ", " + c2);
            Assert.Fail("The column you specified, '{0}', was not in the list of columns found in the grid: [{1}]",
                        columnName,
                        validColumns);
            return null;
        }

        private void ActionWhenITypeIntoTheGridCell(string text,
                                                    int cellIndex)
        {
            WhenFormat(@"I type '{0}' into grid cell {1}",
                       text,
                       cellIndex);
        }

        [When(@"I type '(.*)' into grid cell (\d+).*")]
        public void WhenITypeIntoTheGridCell(string text,
                                             int cellIndex)
        {
            var row = Context
                .Grid
                .SelectedRows
                .First();
            var cell = row.Get(SearchCriteria.ByClassName(typeof (DataGridCell).Name)
                                             .AndIndex(cellIndex));
            cell
                .Should()
                .NotBeNull();
            cell.Enter(text);
        }

        [Then(@"grid column '(.*)', row (.*) has contents '(.*)'")]
        public void ThenGridColumnRowHasContents(string columnName,
                                                 int rowIndex,
                                                 string expectedContent)
        {
            var headerColumn = GetHeaderColumn(columnName);
            ActionThenGridRowCellHasContents(rowIndex,
                                             headerColumn.Index,
                                             expectedContent);
        }

        private void ActionThenGridRowCellHasContents(int rowIndex,
                                                      int columnIndex,
                                                      string expectedContent)
        {
            ThenFormat(@"grid row {0}, cell {1} has contents '{2}'",
                       rowIndex,
                       columnIndex,
                       expectedContent);
        }

        [Then(@"grid row (\d+), cell (\d+) has contents '(.*)'")]
        public void ThenGridRowCellHasContents(int rowIndex,
                                               int columnIndex,
                                               string expectedContent)
        {
            var row = GetRow(rowIndex);
            var cell = row.Get(SearchCriteria.ByClassName(typeof (DataGridCell).Name)
                                             .AndIndex(columnIndex));
            cell.Should()
                .NotBeNull();
            cell.Name
                .Should()
                .Be(expectedContent);
        }

        [Then(@"there is a masked textbox in column '(.*)' on row (.*)")]
        public void ThenThereIsAMaskedTextboxInColumnOnRow(string columnName,
                                                           int rowIndex)
        {
            var headerColumn = GetHeaderColumn(columnName);
            ActionThenThereIsAMaskedTextboxInColumnOnRow(headerColumn.Index,
                                                         rowIndex);
        }

        private void ActionThenThereIsAMaskedTextboxInColumnOnRow(int columnIndex,
                                                                  int rowIndex)
        {
            ThenFormat(@"there is a masked textbox in column {0} on row {1}",
                       columnIndex,
                       rowIndex);
        }

        [Then(@"there is a masked textbox in column (\d+) on row (.*)")]
        public void ThenThereIsAMaskedTextboxInColumnOnRow(int columnIndex,
                                                           int rowIndex)
        {
            var box = FindWidgetIn<TextBox>(columnIndex,
                                            rowIndex);
            Context.TextBox = box;
        }
    }
}