import React, { useState, useEffect } from "react";
import { Box } from "@mui/material";
import { getEmail, getUserThreshold } from "../../ApiCalls/login";
import Loader from "../../Common/Loader";
import "./ProfileDataManager.css";

interface ProfileDataManagerProps {
  children: (data: {
    email: string;
    threshold: number;
    isThresholdUserSet: boolean;
    isLoading: boolean;
    onEmailUpdate: (newEmail: string) => void;
    onThresholdUpdate: (newThreshold: number, isUserSet: boolean) => void;
  }) => React.ReactNode;
}

export default function ProfileDataManager({
  children,
}: ProfileDataManagerProps) {
  const [email, setEmail] = useState<string>("");
  const [threshold, setThreshold] = useState<number>(0);
  const [isThresholdUserSet, setIsThresholdUserSet] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadProfileData();
  }, []);

  const loadProfileData = async () => {
    setIsLoading(true);
    try {
      const [emailResult, thresholdResult] = await Promise.all([
        getEmail(),
        getUserThreshold(),
      ]);

      if (emailResult?.email) {
        setEmail(emailResult.email);
      }

      if (thresholdResult?.threshold !== undefined) {
        setThreshold(thresholdResult.threshold);
        setIsThresholdUserSet(thresholdResult.isUserSet);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleEmailUpdate = (newEmail: string) => {
    setEmail(newEmail);
  };

  const handleThresholdUpdate = (newThreshold: number, isUserSet: boolean) => {
    setThreshold(newThreshold);
    setIsThresholdUserSet(isUserSet);
  };

  if (isLoading) {
    return (
      <Box className="profile-container">
        <Loader />
      </Box>
    );
  }

  return (
    <>
      {children({
        email,
        threshold,
        isThresholdUserSet,
        isLoading,
        onEmailUpdate: handleEmailUpdate,
        onThresholdUpdate: handleThresholdUpdate,
      })}
    </>
  );
}
