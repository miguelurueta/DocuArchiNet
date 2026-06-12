import { useCallback, useEffect, useMemo, useState } from "react";
import { AppDigitalizador } from "../Components/UI/AppDigitalizador";
import type {
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionResult,
} from "../../modules/digitalizacion";
import { debugDynamsoftLicense } from "../../modules/digitalizacion/infrastructure/dynamsoft/dynamsoftLicenseDebug";
import styles from "./AppDigitalizadorSandboxPage.module.css";

const initialContext: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "DOCUARCHI_SANDBOX",
  radicado: "SANDBOX-DIGITALIZACION",
  requiereMetadata: true,
  sourceModule: "Sandbox",
};

export default function AppDigitalizadorSandboxPage() {
  const [result, setResult] = useState<DigitalizacionResult | null>(null);
  const [error, setError] = useState<DigitalizacionDocumentalError | null>(null);
  const [mode, setMode] = useState<DigitalizacionContext["modo"]>("crear");
  const dynamsoftLicenseFromEnv = import.meta.env.VITE_DYNAMSOFT_LICENSE_KEY;

  const context = useMemo<DigitalizacionContext>(
    () => ({
      ...initialContext,
      modo: mode,
      idDocumentoDestino: mode === "adjuntar" ? 1 : undefined,
    }),
    [mode],
  );

  const handleCompleted = useCallback((nextResult: DigitalizacionResult) => {
    setResult(nextResult);
    setError(null);
  }, []);

  const handleError = useCallback((nextError: DigitalizacionDocumentalError) => {
    setError(nextError);
  }, []);

  useEffect(() => {
    debugDynamsoftLicense(
      ".env -> import.meta.env.VITE_DYNAMSOFT_LICENSE_KEY",
      dynamsoftLicenseFromEnv,
    );
  }, [dynamsoftLicenseFromEnv]);

  return (
    <main className={styles.page}>
      <aside className={styles.sidePanel}>
        <h1>Sandbox AppDigitalizador</h1>
        <div className={styles.segmented} aria-label="Modo digitalizacion">
          <button
            type="button"
            data-active={mode === "crear"}
            onClick={() => setMode("crear")}
          >
            Crear
          </button>
          <button
            type="button"
            data-active={mode === "adjuntar"}
            onClick={() => setMode("adjuntar")}
          >
            Adjuntar
          </button>
        </div>
        <dl className={styles.statusList}>
          <div>
            <dt>Resultado</dt>
            <dd>{result ? result.accion : "Sin resultado"}</dd>
          </div>
          <div>
            <dt>Error</dt>
            <dd>{error ? error.message : "Sin error"}</dd>
          </div>
        </dl>
      </aside>

      <section className={styles.digitalizadorPanel}>
        <AppDigitalizador
          context={context}
          modulo="Sandbox"
          licenciaDynamsoft={dynamsoftLicenseFromEnv}
          onCompleted={handleCompleted}
          onError={handleError}
        />
      </section>
    </main>
  );
}
