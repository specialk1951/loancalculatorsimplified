import SwiftUI

@main
struct loancalculatorsimplifiedApp: App {
    @StateObject private var authService = AuthenticationService()

    var body: some Scene {
        WindowGroup {
            if authService.isAuthenticated {
                CalculatorView()
                    .environmentObject(authService)
            } else {
                LoginView()
                    .environmentObject(authService)
            }
        }
    }
}
