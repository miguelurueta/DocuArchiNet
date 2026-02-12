import { readdir, readFile } from "node:fs/promises";
import path from "node:path";

export type TestScanResult = {
  specIds: Set<string>;
  byFile: Record<string, string[]>;
  totalTaggedTests: number;
};

const SPEC_TAG_REGEX = /\[SPEC:([A-Z]+-\d+)\]/g;
const TEST_FILE_REGEX = /\.test\.tsx?$/;

const collectFiles = async (dirPath: string): Promise<string[]> => {
  const entries = await readdir(dirPath, { withFileTypes: true });
  const nested = await Promise.all(
    entries.map(async entry => {
      const fullPath = path.join(dirPath, entry.name);

      if (entry.isDirectory()) {
        return collectFiles(fullPath);
      }

      if (entry.isFile() && TEST_FILE_REGEX.test(entry.name)) {
        return [fullPath];
      }

      return [];
    })
  );

  return nested.flat();
};

const extractSpecTagsFromContent = (content: string): string[] => {
  const ids: string[] = [];

  for (const match of content.matchAll(SPEC_TAG_REGEX)) {
    const id = match[1];
    if (id) {
      ids.push(id);
    }
  }

  return ids;
};

export const extractTestCases = async (projectRoot: string): Promise<TestScanResult> => {
  const targets = [path.resolve(projectRoot, "src/modules/dashboard")];

  const allFiles = (
    await Promise.all(
      targets.map(async dirPath => {
        try {
          return await collectFiles(dirPath);
        } catch {
          return [];
        }
      })
    )
  ).flat();

  const byFile: Record<string, string[]> = {};
  const specIds = new Set<string>();
  let totalTaggedTests = 0;

  await Promise.all(
    allFiles.map(async filePath => {
      const content = await readFile(filePath, "utf-8");
      const tags = extractSpecTagsFromContent(content);

      if (tags.length === 0) {
        return;
      }

      const relativePath = path.relative(projectRoot, filePath);
      byFile[relativePath] = tags;

      tags.forEach(id => specIds.add(id));
      totalTaggedTests += tags.length;
    })
  );

  return {
    specIds,
    byFile,
    totalTaggedTests,
  };
};
