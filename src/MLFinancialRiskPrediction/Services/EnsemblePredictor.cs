namespace MLFinancialRiskPrediction.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ML;

    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Models;

    public class EnsemblePredictor : IEnsemblePredictor
    {
        public EnsemblePredictor([FromKeyedServices("FinancialRiskV2")] MLContext context, ITransformer riskScoreModel, ITransformer loanApprovalModel, ITransformer featureTransformer)
        {
            this._context = context;

            // Create complete pipelines that include feature transformation
            Microsoft.ML.Data.TransformerChain<ITransformer> riskScorePipeline = featureTransformer.Append(riskScoreModel);
            Microsoft.ML.Data.TransformerChain<ITransformer> loanApprovalPipeline = featureTransformer.Append(loanApprovalModel);

            // Create prediction engines with complete pipelines
            this._riskScoreEngine = this._context.Model.CreatePredictionEngine<LoanData, RiskScorePrediction>(riskScorePipeline);
            this._loanApprovalEngine = this._context.Model.CreatePredictionEngine<LoanData, LoanApprovalPrediction>(loanApprovalPipeline);
        }

        #region Private Fields
        private readonly MLContext _context;
        private readonly PredictionEngine<LoanData, RiskScorePrediction> _riskScoreEngine;
        private readonly PredictionEngine<LoanData, LoanApprovalPrediction> _loanApprovalEngine;
        #endregion

        /// <summary>
        /// Predicts the risk score and loan approval status for a single loan data input.
        /// </summary>
        /// <param name="input">The loan data input for which predictions are to be made.</param>
        /// <returns>
        /// An <see cref="EnsemblePrediction"/> containing the risk score and loan approval status.
        /// </returns>
        public EnsemblePrediction Predict(LoanData input)
        {
            RiskScorePrediction riskPrediction = this._riskScoreEngine.Predict(input);
            LoanApprovalPrediction approvalPrediction = this._loanApprovalEngine.Predict(input);

            return new EnsemblePrediction
            {
                RiskScore = riskPrediction.Score,
                LoanApproved = approvalPrediction.Prediction,
                ApprovalProbability = approvalPrediction.Probability,
                ApprovalScore = approvalPrediction.Score
            };
        }

        /// <summary>
        /// Predicts the risk score and loan approval status for a batch of loan data inputs.
        /// </summary>
        /// <param name="inputs">The batch of loan data inputs for which predictions are to be made.</param>
        /// <returns>
        /// An <see cref="IEnumerable{EnsemblePrediction}"/> containing the predictions for each input.
        /// </returns>
        public IEnumerable<EnsemblePrediction> PredictBatch(IEnumerable<LoanData> inputs)
        {
            return inputs.Select(this.Predict);
        }

        /// <summary>
        /// Disposes of the prediction engines and releases any resources they hold.
        /// </summary>
        public void Dispose()
        {
            this._riskScoreEngine?.Dispose();
            this._loanApprovalEngine?.Dispose();
        }
    }
}
