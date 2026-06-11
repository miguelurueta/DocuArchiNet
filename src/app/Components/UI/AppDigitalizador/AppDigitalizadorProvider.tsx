import {
  AppDigitalizadorContext,
  defaultAppDigitalizadorProviderValue,
} from "./AppDigitalizador.context";
import type {
  AppDigitalizadorProviderProps,
} from "./AppDigitalizador.types";

export function AppDigitalizadorProvider({
  children,
  apiClient,
  dynamsoft,
  createScannerClient,
}: AppDigitalizadorProviderProps) {
  return (
    <AppDigitalizadorContext.Provider
      value={{
        apiClient,
        dynamsoft,
        createScannerClient:
          createScannerClient ?? defaultAppDigitalizadorProviderValue.createScannerClient,
      }}
    >
      {children}
    </AppDigitalizadorContext.Provider>
  );
}
