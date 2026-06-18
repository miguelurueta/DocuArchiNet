import { useState } from "react";
import type {
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
} from "../../../../../modules/digitalizacion";
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

  const [resolvedScannerClient] = useState<DigitalizacionScannerClient | undefined>(() => {
    if (scannerClient) {
      return scannerClient;
    }

    const resolvedLicenseKey =
      licenciaDynamsoft ??
      dynamsoft?.licenseKey ??
      providerDynamsoft?.licenseKey;

    const runtimeOptions: DynamsoftRuntimeOptions = {
      ...providerDynamsoft,
      ...dynamsoft,
      licenseKey: resolvedLicenseKey,
    };

    return createScannerClient?.(runtimeOptions);
  });

  return resolvedScannerClient;
};
