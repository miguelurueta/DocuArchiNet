import type { DYNAMSOFT_ALLOWED_COLOR_MODES } from "./dynamsoft.constants";

export type ScannerDevice = {
  id: string;
  name: string;
};

export type ScanPage = {
  id: string;
  index: number;
  thumbnailUrl?: string;
};

export type ScanColorMode = (typeof DYNAMSOFT_ALLOWED_COLOR_MODES)[number];

export type ScanOptions = {
  deviceId: string;
  resolutionDpi?: number;
  colorMode?: ScanColorMode;
  duplex?: boolean;
  removeBlankPages?: boolean;
};

export type PdfGenerationResult = {
  file: File;
  pageCount: number;
};

export interface DigitalizacionScannerClient {
  initialize(): Promise<void>;
  listDevices(): Promise<ScannerDevice[]>;
  selectDevice(deviceId: string): Promise<void>;
  scan(options: ScanOptions): Promise<ScanPage[]>;
  rotatePage(pageId: string, degrees: 90 | 180 | 270): Promise<void>;
  removePage(pageId: string): Promise<void>;
  clear(): Promise<void>;
  generatePdf(fileName: string): Promise<PdfGenerationResult>;
  dispose(): Promise<void>;
}

export type DynamsoftRuntimeOptions = {
  scriptSrc?: string;
  resourcesPath?: string;
  licenseKey?: string;
  containerId?: string;
  documentRef?: Document;
  windowRef?: Window;
};

export type DynamsoftWebTwainFactory = {
  ProductKey?: string;
  ResourcesPath?: string;
  Containers?: Array<{
    WebTwainId: string;
    ContainerId: string;
    Width?: string;
    Height?: string;
  }>;
  Load(): void;
  Unload?(): void;
  GetWebTwain(id: string): DynamsoftWebTwainObject | null;
};

export type DynamsoftWebTwainObject = {
  SourceCount?: number;
  HowManyImagesInBuffer?: number;
  SelectSourceByIndex(index: number): boolean;
  GetSourceNameItems(index: number): string;
  OpenSource(): void;
  CloseSource?(): void;
  AcquireImage(
    options: DynamsoftAcquireOptions,
    onSuccess: () => void,
    onFailure: (code: number, message: string) => void,
  ): void;
  RemoveImage(index: number): void;
  RemoveAllImages(): void;
  Rotate(index: number, degrees: number, keepSize: boolean): boolean;
  ConvertToBlob(
    indices: number[],
    type: DynamsoftImageType,
    onSuccess: (blob: Blob) => void,
    onFailure: (code: number, message: string) => void,
  ): void;
};

export type DynamsoftAcquireOptions = {
  IfShowUI: boolean;
  PixelType: number;
  Resolution: number;
  IfFeederEnabled: boolean;
  IfDuplexEnabled: boolean;
  IfDisableSourceAfterAcquire: boolean;
};

export type DynamsoftImageType = "application/pdf";

export type DynamsoftWindow = Window & {
  Dynamsoft?: {
    DWT?: DynamsoftWebTwainFactory;
  };
};
