export type LocalImage = {
  id: string;
  fileName: string;
  contentType: string;
  size: number;
  blob: Blob;
  createdAt: number;
  documentDraftId?: string;
  sessionId?: string;
};

export type LocalImageScope = {
  documentDraftId?: string;
  sessionId?: string;
};
