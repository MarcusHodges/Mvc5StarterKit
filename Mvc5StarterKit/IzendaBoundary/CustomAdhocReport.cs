﻿using Izenda.BI.Framework.CustomConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Izenda.BI.Framework.Models;
using Izenda.BI.Framework.Models.Common;
using System.ComponentModel.Composition;
using Izenda.BI.Framework.Models.ReportDesigner;
using Izenda.BI.Framework.Components.QueryExpressionTree;
using Izenda.BI.Framework.Utility;

namespace Mvc5StarterKit.IzendaBoundary
{
    /// <summary>
    /// This is the Custom AdhocExtension, it will override the lookup filter values, hidden filter, ....
    /// </summary>
    [Export(typeof(IAdHocExtension))]
    public class CustomAdhocReport : DefaultAdHocExtension
    {
        /// <summary>
        /// Override lookup filter data.
        /// 
        /// Requirement summary:
        /// If logged user has role VP --> show all region
        /// If logged user has role Manager --> show some regions ("South America", "North America")
        /// If logged user has role Employee --> show only one region ("South America")
        /// </summary>
        /// <param name="filterField"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override List<string> OnPostLoadFilterData(ReportFilterField filterField, List<string> data)
        {
            // override dropdown value based on user role for filter on view "OrderDetailsByRegion" and field "CountryRegionName"
            if (filterField.SourceDataObjectName == "OrderDetailsByRegion" && filterField.SourceFieldName == "CountryRegionName"
                && (HttpContext.Current.User.IsInRole("Manager") || HttpContext.Current.User.IsInRole("Employee")))
            {
                ///Clear existing value
                data.Clear();

                // override dropdown's value based on User role

                //Manager, dropdown show "South America" and "North America"
                if (HttpContext.Current.User.IsInRole("Manager"))
                {
                    data.Add("South America");
                    data.Add("North America");
                }

                // Employee, dropdown show "South America" only.
                if (HttpContext.Current.User.IsInRole("Employee"))
                {
                    data.Add("South America");
                }
            }
            return base.OnPostLoadFilterData(filterField, data);
        }

        /// <summary>
        /// Override lookup filter data tree.
        /// 
        /// Requirement summary:
        /// If logged user has role VP --> show all region
        /// If logged user has role Manager --> show some regions ("South America", "North America")
        /// If logged user has role Employy --> show only one region ("South America")
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override List<ValueTreeNode> OnLoadFilterDataTree(QuerySourceFieldInfo fieldInfo)
        {
            var result = new List<ValueTreeNode>();

            if (fieldInfo.QuerySourceName == "OrderDetailsByRegion" && fieldInfo.Name == "CountryRegionName"
                && (HttpContext.Current.User.IsInRole("Manager") || HttpContext.Current.User.IsInRole("Employee")))
            {
                //Node [All] and [Blank] are required for UI to render.
                var rootNode = new ValueTreeNode { Text = "[All]", Value = "[All]" };
                rootNode.Nodes = new List<ValueTreeNode>();
                //rootNode.Nodes.Add(new ValueTreeNode { Text = "[Blank]", Value = "[Blank]" });


                if (HttpContext.Current.User.IsInRole("Manager"))
                {
                    rootNode.Nodes.Add(new ValueTreeNode { Text = "South America", Value = "South America" });
                    rootNode.Nodes.Add(new ValueTreeNode { Text = "North America", Value = "North America" });
                }

                if (HttpContext.Current.User.IsInRole("Employee"))
                {
                    rootNode.Nodes.Add(new ValueTreeNode { Text = "South America", Value = "South America" });
                }

                //else
                //{
                //    rootNode.Nodes.Add(new ValueTreeNode { Text = "Europe", Value = "Europe" });
                //    rootNode.Nodes.Add(new ValueTreeNode { Text = "South America", Value = "South America" });
                //    rootNode.Nodes.Add(new ValueTreeNode { Text = "North America", Value = "North America" });
                //}

                result.Add(rootNode);
            }

            return result;
        }

        public override ReportFilterSetting SetHiddenFilters(SetHiddenFilterParam param)
        {
            var filterFieldName = "ShipRegion";

            Func<ReportFilterSetting, int, QuerySource, QuerySourceField, Guid, Relationship, int> addHiddenFilters = (result, filterPosition, querySource, field, equalOperator, rel) =>
            {
                var firstFilter = new ReportFilterField
                {
                    Alias = $"ShipRegion{filterPosition}",
                    QuerySourceId = querySource.Id,
                    SourceDataObjectName = querySource.Name,
                    QuerySourceType = querySource.Type,
                    QuerySourceFieldId = field.Id,
                    SourceFieldName = field.Name,
                    DataType = field.DataType,
                    Position = ++filterPosition,
                    OperatorId = equalOperator,
                    Value = "WA",
                    RelationshipId = rel?.Id,
                    IsParameter = false,
                    ReportFieldAlias = null
                };

                var secondFilter = new ReportFilterField
                {
                    Alias = $"ShipRegion{filterPosition}",
                    QuerySourceId = querySource.Id,
                    SourceDataObjectName = querySource.Name,
                    QuerySourceType = querySource.Type,
                    QuerySourceFieldId = field.Id,
                    SourceFieldName = field.Name,
                    DataType = field.DataType,
                    Position = ++filterPosition,
                    OperatorId = equalOperator,
                    Value = "[Blank]",
                    RelationshipId = rel?.Id,
                    IsParameter = false,
                    ReportFieldAlias = null
                };
                result.FilterFields.Add(firstFilter);
                result.FilterFields.Add(secondFilter);

                var logic = $"({filterPosition - 1} OR {filterPosition})";
                if (string.IsNullOrEmpty(result.Logic))
                {
                    result.Logic = logic;
                }
                else
                {
                    result.Logic += $" AND {logic}";
                }

                return filterPosition;
            };

            var filterSetting = new ReportFilterSetting()
            {
                FilterFields = new List<ReportFilterField>()
            };
            var position = 0;

            var ds = param.ReportDefinition.ReportDataSource;

            // Build the hidden filters for ship country fields
            foreach (var querySource in param.QuerySources // Scan thru the query sources that are involved in the report
                .Where(x => x.QuerySourceFields.Any(y => y.Name.Equals(filterFieldName, StringComparison.OrdinalIgnoreCase)))) // Take only query sources that have filter field name
            {
                // Pick the relationships that joins the query source as primary source
                // Setting the join ensure the proper table is assigned when using join alias in the UI
                var rels = param.ReportDefinition.ReportRelationship.
                    Where(x => x.JoinQuerySourceId == querySource.Id)
                    .ToList();

                // Find actual filter field in query source
                var field = querySource.QuerySourceFields.FirstOrDefault(x => x.Name.Equals(filterFieldName, StringComparison.OrdinalIgnoreCase));

                // Pick the equal operator
                var equalOperator = Izenda.BI.Framework.Enums.FilterOperator.FilterOperator.EqualsManualEntry.GetUid();

                // In case there is no relationship that the query source is joined as primary
                if (rels.Count() == 0)
                {
                    // Just add hidden filter with null relationship
                    position = addHiddenFilters(filterSetting, position, querySource, field, equalOperator, null);
                }
                else
                {
                    // Loop thru all relationships that the query source is joined as primary and add the hidden field associated with each relationship
                    foreach (var rel in rels)
                    {
                        position = addHiddenFilters(filterSetting, position, querySource, field, equalOperator, rel);
                    }
                }
            }

            return filterSetting;
        }
    }
}