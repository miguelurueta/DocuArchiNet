export type SpecModuleConfig = {
  module: string;
  behaviorSpecs: string[];
  contractSpecs: string[];
  testGlobs: string[];
};

export const SPEC_TAG_REGEX = /\[SPEC:([A-Z]+-\d+)\]/g;

export const SPEC_MODULES: SpecModuleConfig[] = [
  {
    module: "auth",
    behaviorSpecs: ["openspec/auth.behavior.yaml"],
    contractSpecs: ["openspec/auth.contract.yaml"],
    testGlobs: [
      "src/modules/login/**/*.test.ts",
      "src/modules/login/**/*.test.tsx",
      "src/modules/auth/**/*.test.ts",
      "src/modules/auth/**/*.test.tsx",
      "src/app/auth/**/*.test.ts",
      "src/app/auth/**/*.test.tsx",
    ],
  },
  {
    module: "otp",
    behaviorSpecs: ["openspec/otp.behavior.yaml"],
    contractSpecs: ["openspec/otp.contract.yaml"],
    testGlobs: [
      "src/modules/OTP/**/*.test.ts",
      "src/modules/OTP/**/*.test.tsx",
    ],
  },
  {
    module: "dashboard",
    behaviorSpecs: ["openspec/dashboard.behavior.yaml"],
    contractSpecs: ["openspec/dashboard.contract.yaml"],
    testGlobs: [
      "src/modules/dashboard/**/*.test.ts",
      "src/modules/dashboard/**/*.test.tsx",
    ],
  },
];
