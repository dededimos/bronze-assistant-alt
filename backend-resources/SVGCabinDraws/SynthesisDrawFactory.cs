using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCabinDraws
{
    public class SynthesisDrawFactory
    {
        private readonly GlassesBuilderDirector glassBuilder;
        private readonly CabinValidator validator;

        public SynthesisDrawFactory(GlassesBuilderDirector glassBuilder , CabinValidator validator)
        {
            this.glassBuilder = glassBuilder;
            this.validator = validator;
        }

        /// <summary>
        /// Returns the Draw of the selected synthesis
        /// </summary>
        /// <param name="synthesis">The Synthesis to Draw</param>
        /// <returns>The Draw</returns>
        public SynthesisDraw CreateSynthesisDraw(CabinSynthesis synthesis)
        {
            return new(synthesis, glassBuilder, validator);
        }

    }
}
