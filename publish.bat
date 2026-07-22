dotnet clean LoanCalculatorSimplified.csproj -f net9.0-android36.0 -c Release
dotnet publish LoanCalculatorSimplified.csproj -f net9.0-android36.0 -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore="C:\Users\john_\specialk9-release.jks" -p:AndroidSigningKeyAlias=specialk9-key -p:AndroidSigningKeyPass=loancalc2026 -p:AndroidSigningStorePass=loancalc2026
