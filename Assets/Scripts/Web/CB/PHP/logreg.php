<?php
include "db.php";

$dt = $_POST;


$playerData = array(
    "id" => -1,
    "wallet" => "Wallet_Addres",
    "mint" => "Mint_key",
    "participate" => false,
    "score" => ""
);

$error = array(
    "errorText" => "empty",
    "isError" => false
);

$userData = array(
    "playerData" => $playerData,
    "error" => $error
);

if ($dt['type'] == "logging") {
    if (isset($dt['wallet_address'])) {
        $users = $db->query("SELECT * FROM `users` WHERE `wallet_address` = '{$dt['wallet_address']}'");


        if ($users->rowCount() == 1) {
            $user = $users->fetch(PDO::FETCH_ASSOC);

            if ($dt['wallet_address'] == $user['wallet_address']) {

                $data = $db->query("SELECT * FROM `data` WHERE `userid` = {$user['id']}")->fetch(PDO::FETCH_ASSOC);


                $userData["playerData"]["id"] = $user['id'];
                $userData["playerData"]["Mint_key"] = $data['Mint_keys'];
            } else {
                SetError("Wallet is not recognized");
            }
        } else {
            SetError("Wallet is not recognized");
        }
    }
} else
    if ($dt['type'] == "register") {
        if (isset($dt['wallet_address'])) {
            $users = $db->query("SELECT * FROM `users` WHERE `wallet_address` = '{$dt['wallet_address']}'");
            if ($users->rowCount() == 0) {
                //Создание пользователя
                $db->query("INSERT INTO `users`(`wallet_address`) VALUES ('{$dt['wallet_address']}')");
                //Создание данных об игроке
                $db->query("INSERT INTO `data`(`userID`, `levels_score`, `levels_completed`) VALUES ({$db->lastInsertId()}, 0, 0)");
            } else {
                SetError("User Exists");
            }
        }
    } else
        if ($dt['type'] == "save") {
            if (isset($dt['id']) && isset($dt['wallet_address']) && isset($dt['level']) && isset($dt['levelScore'])) {
                $db->query("UPDATE `data` SET `levels_score`='{$dt['level']}',`levels_score`='{$dt['level']}',`levels_score`='{$dt['level']}' WHERE `userid` = {$dt['id']}");
            } else {
                SetError("Save Data Error");
            }
        } else
            if ($dt['type'] == "mintCheck") {
                if (isset($dt['id']) && isset($dt['mint'])) {
                    $mints = $db->query("SELECT * FROM `mints` WHERE `mint` = '{$dt['mint']}'");
                    if ($mints->rowCount() == 0) {
                        $db->query("INSERT INTO `mints`(`userID`, `mint`, `firstHolder`) VALUES ('{$dt['id']}', '{$dt['mint']}', '{$dt['id']}')");
                    } else {
                        $db->query("UPDATE `mints` SET `userID`='{$dt['id']}' WHERE `mint`='{$dt['mint']}'");
                    }
                } else {
                    SetError("Check Mint Error");
                }
            } else
                if ($dt['type'] == "mintUpdate") {
                    if (isset($dt['id'])) {
                        $mints = $db->query("SELECT * FROM `mints` WHERE `userID` = '{$dt['id']}'");
                        if ($mints->rowCount() == 0) {
                            SetError("Player doesnt have any NFT");
                        } else {
                            $db->query("UPDATE `mints` SET `userID`= 0 WHERE `userID`='{$dt['id']}'");
                        }
                    } else {
                        SetError("Update Mint Error");
                    }
                } else if ($dt['type'] == "participate") {
                    if (isset($dt['id'])) {
                        $mints = $db->query("SELECT * FROM `mints` WHERE `firstHolder` = '{$dt['id']}' AND `userid` = '{$dt['id']}'");
                        if ($mints->rowCount() > 0) {
                            $userData["playerData"]["participate"] = true;
                        } else {
                            $userData["playerData"]["participate"] = false;
                            SetError("Player doesnt have unused nft");
                        }
                    }
                } else if ($dt['type'] == "saveLevel") {
                    if (isset($dt['id']) && isset($dt['index']) && isset($dt['score']) && isset($dt['win']) && isset($dt['loose']) && isset($dt['time'])) {

                        $levels = $db->query("SELECT * FROM `level` WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                        $scoreboard = $db->query("SELECT * FROM `scoreboard` WHERE `userid` = '{$dt['id']}'");
                        $time = floatval($dt['time']);

                        if ($scoreboard->rowCount() == 0) {
                            $db->query("INSERT INTO `scoreboard`(`userid`, `levels_sum`, `moves_sum`, `time_sum`, `summary`) VALUES ('{$dt['id']}', '0', '0', '0', '0')");
                        }

                        if ($levels->rowCount() == 0) {
                            //$db->query("UPDATE `scoreboard` SET `levels_sum`=`levels_sum` + 1 WHERE `userid` = '{$dt['id']}'");
                            if ($dt['win'] == 1) {
                                $db->query("INSERT INTO `level`(`userid`, `index`, `score`, `best_score`, `attempts`, `wins`, `looses`, `best_time`, `last_time`, `summary`) VALUES ('{$dt['id']}', '{$dt['index']}', '{$dt['score']}', '{$dt['score']}', '1', '{$dt['win']}', '{$dt['loose']}', '$time', '$time', '{$dt['score']}'/'$time')");
                                $db->query("UPDATE `scoreboard` SET `moves_sum` = `moves_sum` + '{$dt['score']}',`time_sum`=`time_sum`+'{$dt['time']}' WHERE `userid` = '{$dt['id']}'");
                            }
                            if ($dt['loose'] == 1) {
                                $db->query("INSERT INTO `level`(`userid`, `index`, `score`, `attempts`, `wins`, `looses`, `last_time`, `summary`) VALUES ('{$dt['id']}', '{$dt['index']}', '{$dt['score']}', '1', '{$dt['win']}', '{$dt['loose']}', '$time', '0')");
                            }

                        } else {
                            $db->query("UPDATE `level` SET `score`='{$dt['score']}', `attempts` = `attempts` + 1,`wins`=`wins`+'{$dt['win']}',`looses`=`looses`+'{$dt['loose']}',`last_time`='{$dt['time']}' WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");

                            $q = $db->prepare("SELECT `best_score` FROM `level` WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            $q->execute();
                            $BestScore = $q->fetchColumn();

                            $q = $db->prepare("SELECT `best_time` FROM `level` WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            $q->execute();
                            $BestTime = $q->fetchColumn();

                            if ($BestScore === NULL && $dt['win'] == 1) {
                                $db->query("UPDATE `level` SET `best_score`='{$dt['score']}', `summary` = '{$dt['score']}'/'$time' WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            }
                            if ($BestScore > $dt['score'] && $dt['win'] == 1) {
                                $db->query("UPDATE `level` SET `best_score`='{$dt['score']}', `summary` = '{$dt['score']}'/'$time' WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            }
                            if ($BestTime === NULL && $dt['win'] == 1) {
                                $db->query("UPDATE `level` SET `best_time` = '$time', `summary` = '{$dt['score']}'/'$time' WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            }
                            if($BestTime > $time && $dt['win'] == 1){
                                $db->query("UPDATE `level` SET `best_time` = '$time', `summary` = '{$dt['score']}'/'$time' WHERE `userid` = '{$dt['id']}' AND `index` = '{$dt['index']}'");
                            }
                        }

                        $q = $db->prepare("SELECT SUM(`best_score`) FROM `level` WHERE `userID` = '{$dt['id']}' AND `wins` > 0");
                        $q->execute();
                        $BestMovesSum = $q->fetchColumn();

                        $q = $db->prepare("SELECT SUM(`best_time`) FROM `level` WHERE `userID` = '{$dt['id']}' AND `wins` > 0");
                        $q->execute();
                        $BestTimeSum = $q->fetchColumn();

                        $q = $db->query("SELECT COUNT(`index`) FROM `level` WHERE `userid` = '{$dt['id']}' AND `wins` > 0");
                        $q->execute();
                        $levelsSum = $q->fetchColumn();

                        $db->query("UPDATE `scoreboard` SET `moves_sum` = '$BestMovesSum', `time_sum` = '$BestTimeSum', `levels_sum` = '$levelsSum'  WHERE `userid` = '{$dt['id']}'");

                        $db->query("UPDATE `level` SET `best_score` = NULL, `best_time` = NULL WHERE `wins` = 0");
                        $q = $db->prepare("SELECT `levels_sum` FROM `scoreboard` WHERE `userid` = '{$dt['id']}'");
                        $q->execute();
                        $levels_sum = $q->fetchColumn();

                        $q = $db->prepare("SELECT SUM(`summary`) FROM `level` WHERE `userid` = '{$dt['id']}'");
                        $q->execute();
                        $summary = $q->fetchColumn();

                        if ($levels_sum > 0) {
                            $db->query("UPDATE `scoreboard` SET `summary` = '$summary' WHERE `userid` = '{$dt['id']}'");
                        }

                    } else {
                        SetError("Wrong data");
                    }
                } else if ($dt['type'] == "getScoreboard") {
                    $summary = $db->query("SELECT `userid`, `wallet_address` FROM `scoreboard` LEFT JOIN `users` ON `users`.`id` = `scoreboard`.`userID` ORDER BY `levels_sum` DESC, `moves_sum` ASC, `time_sum` ASC");
                    if($summary->rowCount() > 0){
                        $data = "";
                        $dataSum = $summary;
                        //$dataSum->fetch(PDO::FETCH_ASSOC);
                        foreach ($dataSum as $value) {
                            $data = $data . strval($value["userid"]) . "&" . strval($value["wallet_address"]) . "|";
                        }
                        $userData["playerData"]["score"] = $data;
                    }

                } else if($dt['type'] == "getScoreboardByIndex"){
                    if(isset($dt['index'])){
                        $summary = $db->query("SELECT `userid`, `wallet_address`, `best_score`, `best_time` FROM `level` LEFT JOIN `users` ON `users`.`id` = `level`.`userID` WHERE `index` = '{$dt['index']}' AND `best_score` IS NOT NULL AND `best_time` IS NOT NULL ORDER BY `level`.`best_score` ASC, `level`.`best_time` ASC,  `date_modified` ASC");
                        if($summary->rowCount() > 0){
                            $data = "";
                            $dataSum = $summary;
                            //$dataSum->fetch(PDO::FETCH_ASSOC);
                            foreach ($dataSum as $value) {
                                $data = $data . strval($value["userid"]) . "&" . strval($value["wallet_address"]) . "&" . strval($value["best_score"]) . "&" . strval($value["best_time"]) . "|";
                            }
                            $userData["playerData"]["score"] = $data;
                        }
                    }
                } else {
                    SetError("Unknown data");
                }


function SetError($text)
{
    global $userData;
    $userData["playerData"] = null;
    $userData["error"]["isError"] = true;
    $userData["error"]["errorText"] = "Error: " . $text;
}

echo json_encode($userData, JSON_UNESCAPED_UNICODE);
?>