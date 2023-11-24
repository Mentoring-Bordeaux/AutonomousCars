/**
 * @file Sets up HTTPS for the application using an ASP.NET HTTPS certificate.
 */

import { existsSync } from "fs";
import { join } from "path";
import { spawn } from "child_process";

const certificateName = process.env.npm_package_name;
const certificateFolder = process.env.APPDATA
  ? `${process.env.APPDATA}/ASP.NET/https`
  : `${process.env.HOME}/.aspnet/https`;

if (!certificateName) {
  console.error(
    "Invalid certificate name. Run this script in the context of an npm script."
  );
  process.exit(-1);
}

const certFilePath = join(certificateFolder, `${certificateName}.pem`);
const keyFilePath = join(certificateFolder, `${certificateName}.key`);

if (!existsSync(certFilePath) || !existsSync(keyFilePath)) {
  spawn(
    "dotnet",
    [
      "dev-certs",
      "https",
      "--export-path",
      certFilePath,
      "--format",
      "Pem",
      "--no-password",
      "--trust",
    ],
    { stdio: "inherit" }
  ).on("exit", (code) => process.exit(code));
} else {
  spawn("dotnet", ["dev-certs", "https", "--trust"], { stdio: "inherit" }).on(
    "exit",
    (code) => process.exit(code)
  );
}
