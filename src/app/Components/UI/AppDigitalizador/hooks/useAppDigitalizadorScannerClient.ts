import { useMemo } from "react";
import type { DynamsoftRuntimeOptions } from "../../../../../modules/digitalizacion";
import { useAppDigitalizadorProvider } from "../AppDigitalizador.context";
import type { AppDigitalizadorProps } from "../AppDigitalizador.types";

export const useAppDigitalizadorScannerClient = ({
  scannerClient,
  dynamsoft,
  licenciaDynamsoft,
}: Pick<AppDigitalizadorProps, "scannerClient" | "dynamsoft" | "licenciaDynamsoft">) => {
  const {
    createScannerClient,
    dynamsoft: providerDynamsoft,
  } = useAppDigitalizadorProvider();

  return useMemo(() => {
    if (scannerClient) {
      return scannerClient;
    }

    const runtimeOptions: DynamsoftRuntimeOptions = {
      ...providerDynamsoft,
      ...dynamsoft,
      licenseKey:
        licenciaDynamsoft ??
        dynamsoft?.licenseKey ??
        providerDynamsoft?.licenseKey,
    };

    return createScannerClient?.(runtimeOptions);
  }, [
    createScannerClient,
    dynamsoft,
    licenciaDynamsoft,
    providerDynamsoft,
    scannerClient,
  ]);
};
