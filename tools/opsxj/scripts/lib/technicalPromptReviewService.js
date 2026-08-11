// Nombre neutral para el flujo legacy. El modulo frontend permanece como
// implementacion compatible mientras se amplian reglas para WebForms/VB.NET.
export {
  applyPromptReviewCorrection as applyTechnicalReviewCorrection,
  buildPromptReviewCorrection as buildTechnicalReviewCorrection,
  reviewFrontendPrompt as reviewTechnicalPrompt,
  testFrontendPromptReview as testTechnicalReview,
} from "./frontendPromptReviewService.js";
