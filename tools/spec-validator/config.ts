export type SpecValidationConfig = {
  openSpecFiles: string[];
  testGlobs: string[];
  specTagRegex: RegExp;
};

export const SPEC_VALIDATION_CONFIG: SpecValidationConfig = {
  openSpecFiles: [
    "openspec/recovery.behavior.yaml",
    "openspec/recovery.contract.yaml",
    "openspec/reset.behavior.yaml",
    "openspec/reset.contract.yaml",
  ],
  testGlobs: [
    "src/modules/RecoveryPassword/**/*.test.ts",
    "src/modules/RecoveryPassword/**/*.test.tsx",
  ],
  specTagRegex: /\[SPEC:([A-Z]+-\d+)\]/g,
};
