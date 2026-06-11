import { createContext, useContext } from "react";
import { DynamsoftTwainClient } from "../../../../modules/digitalizacion";
import type { AppDigitalizadorProviderValue } from "./AppDigitalizador.types";

export const defaultAppDigitalizadorProviderValue: AppDigitalizadorProviderValue = {
  createScannerClient: (options) => new DynamsoftTwainClient(options),
};

export const AppDigitalizadorContext = createContext<AppDigitalizadorProviderValue>(
  defaultAppDigitalizadorProviderValue,
);

export const useAppDigitalizadorProvider = () => useContext(AppDigitalizadorContext);
