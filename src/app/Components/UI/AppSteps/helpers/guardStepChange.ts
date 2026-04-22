import type { AppStepsVariant } from "../AppSteps.types";

type GuardStepChangeInput = {
  currentIndex: number;
  targetIndex: number;
  targetDisabled?: boolean;
  variant: AppStepsVariant;
  validateStep?: (stepIndex: number) => boolean | Promise<boolean>;
};

type GuardStepChangeResult = {
  canMove: boolean;
  showErrorOnCurrent: boolean;
};

export const guardStepChange = async ({
  currentIndex,
  targetIndex,
  targetDisabled,
  variant,
  validateStep,
}: GuardStepChangeInput): Promise<GuardStepChangeResult> => {
  if (targetDisabled || targetIndex === currentIndex) {
    return { canMove: false, showErrorOnCurrent: false };
  }

  if (variant !== "form" || !validateStep) {
    return { canMove: true, showErrorOnCurrent: false };
  }

  const isValid = await Promise.resolve(validateStep(currentIndex));
  if (!isValid) {
    return { canMove: false, showErrorOnCurrent: true };
  }

  return { canMove: true, showErrorOnCurrent: false };
};
