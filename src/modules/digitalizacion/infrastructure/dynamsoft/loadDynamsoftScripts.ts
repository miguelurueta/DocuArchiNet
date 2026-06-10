import {
  DYNAMSOFT_DEFAULT_SCRIPT_SRC,
  DYNAMSOFT_SCRIPT_ID,
} from "./dynamsoft.constants";
import { DynamsoftScannerError } from "./dynamsoft.errors";

let scriptLoadPromise: Promise<void> | null = null;

export const resetDynamsoftScriptLoaderForTests = () => {
  scriptLoadPromise = null;
};

export const loadDynamsoftScripts = ({
  scriptSrc = DYNAMSOFT_DEFAULT_SCRIPT_SRC,
  documentRef = document,
}: {
  scriptSrc?: string;
  documentRef?: Document;
} = {}) => {
  const existing = documentRef.getElementById(DYNAMSOFT_SCRIPT_ID);

  if (existing?.getAttribute("data-loaded") === "true") {
    return Promise.resolve();
  }

  if (scriptLoadPromise) {
    return scriptLoadPromise;
  }

  scriptLoadPromise = new Promise<void>((resolve, reject) => {
    const script =
      existing instanceof HTMLScriptElement
        ? existing
        : documentRef.createElement("script");

    script.id = DYNAMSOFT_SCRIPT_ID;
    script.src = scriptSrc;
    script.async = true;

    script.addEventListener(
      "load",
      () => {
        script.setAttribute("data-loaded", "true");
        resolve();
      },
      { once: true },
    );
    script.addEventListener(
      "error",
      () => {
        scriptLoadPromise = null;
        reject(
          new DynamsoftScannerError({
            code: "DYNAMSOFT_SCRIPT_LOAD_FAILED",
            message: "No fue posible cargar Dynamsoft Web TWAIN.",
          }),
        );
      },
      { once: true },
    );

    if (!existing) {
      documentRef.head.appendChild(script);
    }
  });

  return scriptLoadPromise;
};
