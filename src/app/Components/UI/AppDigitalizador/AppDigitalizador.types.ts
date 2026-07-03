import type { ReactNode } from "react";
import type {
  DigitalizacionApiClient,
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionResult,
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
} from "../../../../modules/digitalizacion";

export type AppDigitalizadorModulo =
  | "CapDocument"
  | "Correspondencia"
  | "Workflow"
  | "Ventanilla"
  | "ArchivoCentral"
  | "Radicacion"
  | "PQRS"
  | "Contratos"
  | "ProduccionDocumental"
  | string;

export type AppDigitalizadorMode = DigitalizacionContext["modo"];

export type AppDigitalizadorScannerFactory = (
  options: DynamsoftRuntimeOptions,
) => DigitalizacionScannerClient;

export type AppDigitalizadorProviderValue = {
  apiClient?: DigitalizacionApiClient;
  dynamsoft?: DynamsoftRuntimeOptions;
  createScannerClient?: AppDigitalizadorScannerFactory;
};

export type AppDigitalizadorProviderProps = AppDigitalizadorProviderValue & {
  children: ReactNode;
};

export type AppDigitalizadorProps = {
  context: DigitalizacionContext | null;
  onCompleted: (result: DigitalizacionResult) => void;
  active?: boolean;
  modulo?: AppDigitalizadorModulo;
  apiClient?: DigitalizacionApiClient;
  scannerClient?: DigitalizacionScannerClient;
  dynamsoft?: DynamsoftRuntimeOptions;
  licenciaDynamsoft?: string;
  className?: string;
  onCancel?: () => void;
  onError?: (error: DigitalizacionDocumentalError) => void;
};
