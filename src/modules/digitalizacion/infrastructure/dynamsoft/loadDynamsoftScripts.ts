import {
  DYNAMSOFT_CSS_ID_PREFIX,
  DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  DYNAMSOFT_DEFAULT_SCRIPT_SRC,
  DYNAMSOFT_REQUIRED_CSS_FILES,
  DYNAMSOFT_SCRIPT_ID,
} from "./dynamsoft.constants";
import { DynamsoftScannerError } from "./dynamsoft.errors";

let scriptLoadPromise: Promise<void> | null = null;
let cssLoadPromise: Promise<void> | null = null;

export const resetDynamsoftScriptLoaderForTests = () => {
  scriptLoadPromise = null;
  cssLoadPromise = null;
};

const normalizeResourcesPath = (resourcesPath: string) => resourcesPath.replace(/\/+$/, "");

export const buildDynamsoftCssUrls = (
  resourcesPath = DYNAMSOFT_DEFAULT_RESOURCES_PATH,
) => {
  const normalizedResourcesPath = normalizeResourcesPath(resourcesPath);

  return DYNAMSOFT_REQUIRED_CSS_FILES.map(
    (fileName) => `${normalizedResourcesPath}/${fileName}`,
  );
};

const getCssElementId = (index: number) => `${DYNAMSOFT_CSS_ID_PREFIX}-${index}`;

const loadStyleSheet = ({
  cssUrl,
  elementId,
  documentRef,
}: {
  cssUrl: string;
  elementId: string;
  documentRef: Document;
}) => {
  const existing = documentRef.getElementById(elementId);

  if (existing?.getAttribute("data-loaded") === "true") {
    return Promise.resolve();
  }

  return new Promise<void>((resolve, reject) => {
    const link =
      existing instanceof HTMLLinkElement ? existing : documentRef.createElement("link");

    link.id = elementId;
    link.rel = "stylesheet";
    link.href = cssUrl;

    link.addEventListener(
      "load",
      () => {
        link.setAttribute("data-loaded", "true");
        resolve();
      },
      { once: true },
    );
    link.addEventListener(
      "error",
      () => {
        reject(
          new DynamsoftScannerError({
            code: "DYNAMSOFT_CSS_LOAD_FAILED",
            message:
              "No fue posible cargar los estilos CSS de Dynamsoft Web TWAIN.",
          }),
        );
      },
      { once: true },
    );

    if (!existing) {
      documentRef.head.appendChild(link);
    }
  });
};

const loadDynamsoftCss = ({
  resourcesPath = DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  documentRef,
}: {
  resourcesPath?: string;
  documentRef: Document;
}) => {
  if (cssLoadPromise) {
    return cssLoadPromise;
  }

  const cssUrls = buildDynamsoftCssUrls(resourcesPath);
  cssLoadPromise = Promise.all(
    cssUrls.map((cssUrl, index) =>
      loadStyleSheet({
        cssUrl,
        elementId: getCssElementId(index),
        documentRef,
      }),
    ),
  )
    .then(() => undefined)
    .catch((error) => {
      cssLoadPromise = null;
      throw error;
    });

  return cssLoadPromise;
};

export const loadDynamsoftScripts = ({
  scriptSrc = DYNAMSOFT_DEFAULT_SCRIPT_SRC,
  resourcesPath = DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  documentRef = document,
}: {
  scriptSrc?: string;
  resourcesPath?: string;
  documentRef?: Document;
} = {}) => {
  const existing = documentRef.getElementById(DYNAMSOFT_SCRIPT_ID);

  if (existing?.getAttribute("data-loaded") === "true") {
    return loadDynamsoftCss({ resourcesPath, documentRef });
  }

  if (scriptLoadPromise) {
    return Promise.all([
      scriptLoadPromise,
      loadDynamsoftCss({ resourcesPath, documentRef }),
    ]).then(() => undefined);
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
        loadDynamsoftCss({ resourcesPath, documentRef }).then(resolve, reject);
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
