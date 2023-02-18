INSERT INTO numbersequence (`sequence`, `number`) VALUES (@Sequence, 1)
    ON DUPLICATE KEY UPDATE `number` = `number` + 1;

select `number` from numbersequence where `sequence` = @Sequence;