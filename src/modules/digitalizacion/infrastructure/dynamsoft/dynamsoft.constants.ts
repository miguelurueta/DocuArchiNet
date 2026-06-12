export const DYNAMSOFT_SCRIPT_ID = "digitalizacion-dynamsoft-webtwain";
export const DYNAMSOFT_CSS_ID_PREFIX = "digitalizacion-dynamsoft-webtwain-css";
export const DYNAMSOFT_SDK_VERSION = "19.3.2";
export const DYNAMSOFT_EXPECTED_SERVICE_VERSION = "1.9.3.1028";
export const DYNAMSOFT_EXPECTED_TWAIN_MODULE_VERSION = "19.3.2";
export const DYNAMSOFT_DEFAULT_RESOURCES_PATH =
  `https://cdn.jsdelivr.net/npm/dwt@${DYNAMSOFT_SDK_VERSION}/dist`;
export const DYNAMSOFT_DEFAULT_SCRIPT_SRC =
  `${DYNAMSOFT_DEFAULT_RESOURCES_PATH}/dynamsoft.webtwain.min.js`;
export const DYNAMSOFT_SERVICE_INSTALLER_URL =
  `${DYNAMSOFT_DEFAULT_RESOURCES_PATH}/dist/DynamicWebTWAINServiceSetup.msi`;
export const DYNAMSOFT_REQUIRED_CSS_FILES = [
  "src/dynamsoft.webtwain.css",
  "src/dynamsoft.webtwain.viewer.css",
] as const;
export const DYNAMSOFT_CONTAINER_ID = "digitalizacion-dynamsoft-container";
export const DYNAMSOFT_ALLOWED_COLOR_MODES = [
  "color",
  "grayscale",
  "blackWhite",
] as const;
export const DYNAMSOFT_DEFAULT_RESOLUTION_DPI = 200;
export const DYNAMSOFT_MIN_RESOLUTION_DPI = 75;
export const DYNAMSOFT_MAX_RESOLUTION_DPI = 600;
