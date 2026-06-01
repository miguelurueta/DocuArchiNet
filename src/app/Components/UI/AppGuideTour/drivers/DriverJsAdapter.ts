import { driver } from "driver.js";
import type { Config, DriveStep, Driver } from "driver.js";
import "driver.js/dist/driver.css";

import type {
  AppGuideTourDriver,
  AppGuideTourDriverCallbacks,
  AppGuideTourStep,
} from "../AppGuideTour.types";

function mapStep(step: AppGuideTourStep): DriveStep {
  return {
    element: step.element,
    popover: {
      title: step.title,
      description: step.description,
      side: step.side ?? "bottom",
      align: "start",
    },
  };
}

export class DriverJsAdapter implements AppGuideTourDriver {
  private instance: Driver | null = null;
  private activeStepIndex = 0;
  private totalSteps = 0;
  private readonly callbacks: AppGuideTourDriverCallbacks;

  constructor(callbacks: AppGuideTourDriverCallbacks) {
    this.callbacks = callbacks;
  }

  start(steps: AppGuideTourStep[]) {
    try {
      this.totalSteps = steps.length;
      this.activeStepIndex = 0;

      const config: Config = {
        steps: steps.map(mapStep),
        animate: true,
        allowClose: true,
        allowKeyboardControl: true,
        overlayClickBehavior: "close",
        showProgress: true,
        stagePadding: 8,
        stageRadius: 7,
        nextBtnText: "Siguiente",
        prevBtnText: "Anterior",
        doneBtnText: "Finalizar",
        progressText: "{{current}}/{{total}}",
        popoverClass: "app-guide-tour-popover",
        onHighlighted: (_element, _step, opts) => {
          const activeIndex = opts.driver.getActiveIndex() ?? 0;
          this.activeStepIndex = activeIndex;
          this.callbacks.onStepChange(activeIndex);
        },
        onDestroyed: () => {
          if (this.totalSteps > 0 && this.activeStepIndex >= this.totalSteps - 1) {
            this.callbacks.onCompleted();
            return;
          }

          this.callbacks.onCancelled();
        },
      };

      if (!this.instance) {
        this.instance = driver(config);
      } else {
        this.instance.setConfig(config);
      }

      this.instance.drive();
    } catch (error) {
      this.callbacks.onError(error instanceof Error ? error.message : "driver_start_failed");
    }
  }

  stop() {
    this.instance?.destroy();
  }

  refresh() {
    this.instance?.refresh();
  }

  destroy() {
    this.instance?.destroy();
    this.instance = null;
  }
}
