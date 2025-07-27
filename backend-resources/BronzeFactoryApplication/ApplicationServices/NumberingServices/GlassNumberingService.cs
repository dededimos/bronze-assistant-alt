using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NumberingServices
{
    /// <summary>
    /// Applies Numbering and Lettering to the Glass Rows of a GlassesOrder
    /// </summary>
    public class GlassNumberingService
    {
        private static readonly string stepLettering = "K";
        private static readonly string roundingLettering = "R";

        public GlassesOrderViewModel? OrderViewModel { get; set; }
        
        public void SetGlassesOrder(GlassesOrderViewModel? glassesOrderViewmodel)
        {
            OrderViewModel = glassesOrderViewmodel;
        }

        /// <summary>
        /// Resets Lettering and Numbering for the Whole Order
        /// </summary>
        private void ResetLetteringNumbering()
        {
            if (OrderViewModel is null || OrderViewModel.CabinsCount < 1) return;
            
            var glassRows = OrderViewModel.CabinRows.SelectMany(r => r.GlassesRows);
            foreach (var glassRow in glassRows)
            {
                glassRow.SpecialDrawNumber = null;
                glassRow.SpecialDrawString = null;
            }
        }

        /// <summary>
        /// Returns all the Numbers Sequence of the Selected Lettering (e.x. all numbers for glassRows with lettering 'K')
        /// </summary>
        /// <param name="lettering">The Lettering</param>
        /// <returns></returns>
        private IEnumerable<int> GetAllNumbersOfLettering(string lettering)
        {
            return OrderViewModel?.GlassRows
                .Where(gr => gr.SpecialDrawNumber is not null and not 0 && gr.SpecialDrawString == lettering)
                .Select(gr => (int)gr.SpecialDrawNumber!) ?? Enumerable.Empty<int>();
        }

        /// <summary>
        /// Returns the Current Number for the selected lettering (for example for K => K1 , K2 e.t.c depending on how many glasses have one)
        /// </summary>
        /// <param name="lettering">The Lettering (ex. 'K' for step , 'R' for Rounding) , Lettering is stored as variable for type-safety</param>
        /// <returns></returns>
        private int GetCurrentMaxNumberOfLettering(string lettering)
        {
            return OrderViewModel?.GlassRows
                  .Where(gr => gr.SpecialDrawString == lettering)
                  .Max(gr => gr.SpecialDrawNumber)
                  ?? 0;
        }

        /// <summary>
        /// Retruns the missing numbers in a number sequence (e.x. '1,3,5' will return '2,4')
        /// </summary>
        /// <param name="ints">The Integers for which we need to fill the gap</param>
        /// <param name="startForm1">Weather the Minimum number should be 1 otherwise the Min of the given sequence</param>
        /// <returns>The Missing Numbers Sequence</returns>
        private IEnumerable<int> GetMissingInBetweenNumbers(IEnumerable<int> ints , bool startForm1)
        {
            if(ints.Any() is false) return Enumerable.Empty<int>();

            int min;
            //The First number should always be 1
            if (startForm1) { min = 1; }
            else min = ints.Min();
            
            int max = ints.Max();
            //Get the all the missing numbers from min up to max except from the numbers already present (for example 1,3,5 will return 2,4)
            var missing = Enumerable.Range(min, max - min + 1).Except(ints);
            return missing;
        }

        /// <summary>
        /// Applies lettering to a single Row
        /// </summary>
        /// <param name="newGlassRow"></param>
        private void SetGlassRowLettering(GlassOrderRow newGlassRow)
        {
            //Apply step Lettering
            if (newGlassRow.OrderedGlass.HasStep)
            {
                newGlassRow.SpecialDrawString = stepLettering;
            }
            //Apply Rounding Lettering
            if (newGlassRow.OrderedGlass.HasRounding && newGlassRow.OrderedGlass.Draw != GlassDrawEnum.DrawNV)
            {
                newGlassRow.SpecialDrawString = newGlassRow.SpecialDrawString is null ? roundingLettering : $"{newGlassRow.SpecialDrawString}{roundingLettering}";
            }
        }

        /// <summary>
        /// Reapplies Lettering and Numbering to All GlassRows of the Order
        /// </summary>
        public void ReApplyOrderGlassLetteringNumbering()
        {
            //Reset all
            ResetLetteringNumbering();
            //Return if there are no Cabins
            if (OrderViewModel is null || OrderViewModel.CabinRows.Any() is false) return;

            //Get All GlassRows
            var glassRows = OrderViewModel.GlassRows;
            
            //Set Lettering to All
            foreach (var row in glassRows)
            {
                SetGlassRowLettering(row);
            }

            //Group The Glasses by Lettering
            var rowsGroups = glassRows.Where(row => !string.IsNullOrEmpty(row.SpecialDrawString))
                                  .GroupBy(row => row.SpecialDrawString);

            //Foreach Lettering Apply Numbering
            foreach (var rowsGroup in rowsGroups)
            {
                int accumulatingNumber = 1;
                foreach (var row in rowsGroup)
                {
                    row.SpecialDrawNumber = accumulatingNumber;
                    accumulatingNumber++;
                }
            }
        }

        /// <summary>
        /// Applyes Lettering and Numbering ONLY to the rows that do not already have one
        /// </summary>
        public void ApplyMissingLetteringNumbering()
        {
            if (OrderViewModel is null || OrderViewModel.CabinRows.Any() is false) return;

            //Select only the Glass Rows that do not have Lettering (those will also not have numbering)
            //We must call toList - otherwise the IEnumerable has defered execution and the Grouping below does not work correctly (revaluates things because of lettering change)
            var glassRowsWithoutLettering = OrderViewModel.GlassRows.Where(gr => string.IsNullOrEmpty(gr.SpecialDrawString)).ToList();

            //Apply Lettering to all those Rows
            foreach (var row in glassRowsWithoutLettering)
            {
                SetGlassRowLettering(row);
            }

            //Group the newly lettered glassRows into Groups with Lettering (Leave Nulls out)
            var rowsGroups = glassRowsWithoutLettering
                .Where(gr=> !string.IsNullOrEmpty(gr.SpecialDrawString))
                .GroupBy(row => row.SpecialDrawString);

            //Apply Numbering to each Group
            foreach (var rowGroup in rowsGroups)
            {
                //Find all the current Numbers for this letter 
                var allNumbers = GetAllNumbersOfLettering(rowGroup.Key!);

                //Find all the Missing in between Numbers for this groups lettering
                List<int> missingNumbers = GetMissingInBetweenNumbers(allNumbers, true).ToList();

                int maxCurrentNumber = GetCurrentMaxNumberOfLettering(rowGroup.Key!);
                int accumulator = 1;
                //Pass to Numbering to each row
                foreach (var row in rowGroup)
                {
                    //If there is any missing number left assign it and remove it from the list
                    if (missingNumbers.Any())
                    {
                        var missingNumberToAssign = missingNumbers.First();
                        row.SpecialDrawNumber = missingNumberToAssign;
                        missingNumbers.Remove(missingNumberToAssign);
                    }
                    else //else accumulate
                    {
                        row.SpecialDrawNumber = maxCurrentNumber + accumulator;
                        accumulator++;
                    }
                }
            }
        }

    }
}
