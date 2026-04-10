package com.unity3d.mediation;

import java.util.Map;

/**
 * Stub for LevelPlayPrivacySettings missing from the current LevelPlay SDK.
 * This prevents the build error in LevelPlayBridge.java.
 * Remove this file when updating LevelPlay to a version that includes this class.
 */
public class LevelPlayPrivacySettings {
    public static void setGDPRConsents(Map<String, Boolean> consents) {
        // No-op stub
    }

    public static void setCCPA(boolean value) {
        // No-op stub
    }

    public static void setCOPPA(boolean value) {
        // No-op stub
    }
}
