import Foundation
import Security
import CryptoKit

class AuthenticationService: ObservableObject {
    @Published var isAuthenticated = false
    @Published var isPinSet = false

    private let pinKey = "com.loancalcsimplified.pin"

    init() {
        isPinSet = getPinHash() != nil
    }

    func setupPin(_ pin: String) -> Bool {
        let hash = hashPin(pin)
        let saved = saveToKeychain(hash)
        if saved {
            isPinSet = true
            isAuthenticated = true
        }
        return saved
    }

    func verifyPin(_ pin: String) -> Bool {
        guard let storedHash = getPinHash() else { return false }
        let inputHash = hashPin(pin)
        let success = storedHash == inputHash
        if success {
            isAuthenticated = true
        }
        return success
    }

    func logout() {
        isAuthenticated = false
    }

    private func hashPin(_ pin: String) -> String {
        let data = Data((pin + "LoanCalcSalt").utf8)
        let hash = SHA256.hash(data: data)
        return hash.compactMap { String(format: "%02x", $0) }.joined()
    }

    private func saveToKeychain(_ value: String) -> Bool {
        let data = Data(value.utf8)

        // Delete existing item first
        let deleteQuery: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrAccount as String: pinKey
        ]
        SecItemDelete(deleteQuery as CFDictionary)

        // Add new item
        let addQuery: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrAccount as String: pinKey,
            kSecValueData as String: data,
            kSecAttrAccessible as String: kSecAttrAccessibleWhenUnlockedThisDeviceOnly
        ]

        let status = SecItemAdd(addQuery as CFDictionary, nil)
        return status == errSecSuccess
    }

    private func getPinHash() -> String? {
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrAccount as String: pinKey,
            kSecReturnData as String: true,
            kSecMatchLimit as String: kSecMatchLimitOne
        ]

        var result: AnyObject?
        let status = SecItemCopyMatching(query as CFDictionary, &result)

        guard status == errSecSuccess,
              let data = result as? Data,
              let hash = String(data: data, encoding: .utf8) else {
            return nil
        }

        return hash
    }
}
