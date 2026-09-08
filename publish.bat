@echo off
if "%LOANCALC_KEYSTORE_PASSWORD%"=="" (
    echo Set LOANCALC_KEYSTORE_PASSWORD before running this script ^(it is the specialk9-release.jks password^).
    exit /b 1
)

dotnet clean LoanCalculatorSimplified.csproj -f net9.0-android36.0 -c Release
dotnet publish LoanCalculatorSimplified.csproj -f net9.0-android36.0 -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore="C:\Users\john_\specialk9-release.jks" -p:AndroidSigningKeyAlias=specialk9-key -p:AndroidSigningKeyPass=%LOANCALC_KEYSTORE_PASSWORD% -p:AndroidSigningStorePass=%LOANCALC_KEYSTORE_PASSWORD%
