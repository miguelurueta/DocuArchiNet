import type { ApiMeta } from "./ApiMeta";
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  meta?: ApiMeta;
  errors?: unknown[];
}