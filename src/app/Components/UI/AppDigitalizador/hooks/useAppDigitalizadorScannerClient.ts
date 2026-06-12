import { useState } from "react";
import type {
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
} from "../../../../../modules/digitalizacion";
import { debugDynamsoftLicense } from "../../../../../modules/digitalizacion/infrastructure/dynamsoft/dynamsoftLicenseDebug";
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

  console.log("SCANNER_CLIENT_DEPENDENCIES", {
    createScannerClient,
    dynamsoft,
    licenciaDynamsoft,
    providerDynamsoft,
    scannerClient,
  });

  const [resolvedScannerClient] = useState<DigitalizacionScannerClient | undefined>(() => {
    if (scannerClient) {
      console.log("APP_DIGITALIZADOR_SCANNER_CLIENT_EXTERNAL", scannerClient);
      return scannerClient;
    }

    const resolvedLicenseKey =
      licenciaDynamsoft ??
      dynamsoft?.licenseKey ??
      providerDynamsoft?.licenseKey;
    debugDynamsoftLicense(
      "useAppDigitalizadorScannerClient.runtimeOptions.licenseKey",
      resolvedLicenseKey,
    );

    const runtimeOptions: DynamsoftRuntimeOptions = {
      ...providerDynamsoft,
      ...dynamsoft,
      licenseKey: resolvedLicenseKey,
    };

    console.log("APP_DIGITALIZADOR_SCANNER_CLIENT_CREATE");
    console.log("APP_DIGITALIZADOR_SCANNER_CLIENT_CREATE_CONTEXT", {
      runtimeOptions,
      createScannerClient,
      providerDynamsoft,
      dynamsoft,
    });
    const createdClient = createScannerClient?.(runtimeOptions);
    console.log("APP_DIGITALIZADOR_SCANNER_CLIENT_CREATED", createdClient);
    return createdClient;
  });

  return resolvedScannerClient;
};
