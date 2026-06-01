import type {
  AppGuideTourDriver,
  AppGuideTourDriverCallbacks,
  AppGuideTourDriverFactory,
} from "./AppGuideTour.types";
import { DriverJsAdapter } from "./drivers/DriverJsAdapter";

export type {
  AppGuideTourDriver,
  AppGuideTourDriverCallbacks,
  AppGuideTourDriverFactory,
} from "./AppGuideTour.types";

export function createDriverJsAdapter(callbacks: AppGuideTourDriverCallbacks): AppGuideTourDriver {
  return new DriverJsAdapter(callbacks);
}

export const defaultAppGuideTourDriverFactory: AppGuideTourDriverFactory = createDriverJsAdapter;
